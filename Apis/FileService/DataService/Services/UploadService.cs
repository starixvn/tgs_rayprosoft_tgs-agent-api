using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TGS.Apis.FileService.DataService.Model;
using TGS.Core.Infrastructures.Utility;
using File = TGS.Apis.FileService.DataService.Model.File;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IUploadService
    {
        Task<string> UploadFileAsync(IFormFile formFile);
        Task<string> UploadFileS3Async(IFormFile formFile);
    }

    public class UploadService : IUploadService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IOptions<AwsSettings> _awsSettings;
        private readonly IRepository<File> _files;
        private readonly IOptions<FileSetting> _fileSetting;
        private readonly IRepository<StationLetterFile> _stationLetters;

        public UploadService(IAmazonS3 amazonS3, IOptions<AwsSettings> awsSettings,
            IOptions<FileSetting> fileSetting, IRepository<StationLetterFile> stationLetters,
            IRepository<File> files)
        {
            _amazonS3 = amazonS3;
            _awsSettings = awsSettings;
            _fileSetting = fileSetting;
            _stationLetters = stationLetters;
            _files = files;
        }

        async Task<string> IUploadService.UploadFileAsync(IFormFile formFile)
        {
            var randomFileName =
                $"{StringGenerator.GenerateAlphanumericString(32)}{Path.GetExtension(formFile.FileName)}";
            var path = Path.Join(_fileSetting.Value.Path, "/", randomFileName);

            var file = new File
            {
                Id = Guid.NewGuid().ToString(),
                FileName = randomFileName,
                ContentType = formFile.ContentType,
                FileLength = formFile.Length
            };

            await using var stream = System.IO.File.Create(path);

            await formFile.CopyToAsync(stream);

            await _files.InsertAsync(file);

            return $"/{path}";
        }

        async Task<string> IUploadService.UploadFileS3Async(IFormFile formFile)
        {
            var randomFileName =
                $"{StringGenerator.GenerateAlphanumericString(32)}{Path.GetExtension(formFile.FileName)}";

            var request = new PutObjectRequest
            {
                BucketName = _awsSettings.Value.S3.Bucket,
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
                Id = Guid.NewGuid().ToString(),
                FileName = randomFileName,
                ContentType = formFile.ContentType,
                FileLength = formFile.Length
            };

            await _files.InsertAsync(file);

            return $"{_awsSettings.Value.S3.ServiceUrl}/{_awsSettings.Value.S3.Bucket}/{randomFileName}";
        }
    }
}