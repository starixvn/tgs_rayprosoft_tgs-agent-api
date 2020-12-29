using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using TGS.Apis.FileService.DataService.Model;
using TGS.Core.Infrastructures.Aws;
using TGS.Core.Infrastructures.Nats.Model;
using TGS.Core.Infrastructures.Utility;
using File = TGS.Apis.FileService.DataService.Model.File;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IUploadService
    {
        Task<ListBucketsResponse> ListBucketsAsync();
        
        Task<string> UploadFileS3Async(IFormFile formFile);
       
        Task AddStationLetterFileAsync(ObjectFileEventModel model);

        Task<string[]> GetObjectsAsync(string bucket, string prefix = null,
            IDictionary<string, object> additionalProperties = null);
    }

    public sealed class UploadService : IUploadService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly AwsSettings _awsSettings;
        private readonly IRepository<File> _fileRepository;
        private readonly IRepository<StationLetterFile> _stationLetterRepository;

        public UploadService(IAmazonS3 amazonS3, AwsSettings awsSettings, IRepository<File> fileRepository,
            IRepository<StationLetterFile> stationLetterRepository)
        {
            _amazonS3 = amazonS3;
            _awsSettings = awsSettings;
            _fileRepository = fileRepository;
            _stationLetterRepository = stationLetterRepository;
        }

        public async Task<string> UploadFileS3Async(IFormFile formFile)
        {
            var randomFileName =
                $"{StringGenerator.GenerateAlphanumericString(32)}{Path.GetExtension(formFile.FileName)}";

            var request = new PutObjectRequest
            {
                BucketName = _awsSettings.S3.Bucket,
                Key = randomFileName,
                InputStream = formFile.OpenReadStream()
            };

            var response = await _amazonS3.PutObjectAsync(request);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"[FileService] Couldn't upload file {formFile.FileName} {formFile.ContentType} {formFile.Length}");
            }

            var file = new File
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FileName = randomFileName,
                ContentType = formFile.ContentType,
                FileLength = formFile.Length,
                CreatedOn = DateTimeOffset.Now
            };

            await _fileRepository.InsertAsync(file);

            return $"{_awsSettings.ServiceUrl}/{_awsSettings.S3.Bucket}/{randomFileName}";
        }

        async Task IUploadService.AddStationLetterFileAsync(ObjectFileEventModel model)
        {
            var filter = Builders<StationLetterFile>.Filter.Eq(x => x.StationLetterId, model.Id);
            var stationLetterFile = await _stationLetterRepository.GetByFilterAsync(filter);
            if (stationLetterFile != null)
            {
                stationLetterFile.Files = model.FileNames;
                stationLetterFile.UpdatedOn = DateTimeOffset.Now;

                await _stationLetterRepository.UpdateAsync(stationLetterFile);
            }
            else
            {
                stationLetterFile = new StationLetterFile
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    StationLetterId = model.Id,
                    Files = model.FileNames,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                };

                await _stationLetterRepository.InsertAsync(stationLetterFile);
            }

            await CleanFiles();
        }

        async Task<string[]> IUploadService.GetObjectsAsync(string bucket, string prefix,
            IDictionary<string, object> additionalProperties) =>
            (await _amazonS3.GetAllObjectKeysAsync(bucket, prefix, additionalProperties)).ToArray();

        Task<ListBucketsResponse> IUploadService.ListBucketsAsync() => _amazonS3.ListBucketsAsync();

        private async Task CleanFiles()
        {
            try
            {
                var stationLetterFileNames =
                    _stationLetterRepository.GetQueries()
                        .SelectMany(l => l.Files)
                        .Select(f => f);

                var files = _fileRepository.GetQueries().Where(x => x.CreatedOn < DateTimeOffset.Now.AddHours(-24))
                    .ToArray();
                var fileNames = files.Select(f => f.FileName);

                var expectedList = fileNames.Except(stationLetterFileNames);

                var enumerable = expectedList as string[] ?? expectedList.ToArray();
                if (!enumerable.Any())
                {
                    return;
                }

                var keysAndVersions = enumerable.Select(key => new KeyVersion
                {
                    Key = key
                }).ToList();

                var request = new DeleteObjectsRequest
                {
                    BucketName = _awsSettings.S3.Bucket,
                    Objects = keysAndVersions
                };

                await _amazonS3.DeleteObjectsAsync(request);
                await _fileRepository.DeleteManyAsync(files.Where(x => enumerable.Contains(x.FileName))
                    .Select(x => x.Id)
                    .ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}