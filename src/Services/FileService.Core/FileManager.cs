using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace TGS.FileService
{
    internal class FileManager : IFileManager
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IFileMetaRepository _fileMetaRepository;
        private AwsOptions _options;

        public FileManager(
            IOptions<AwsOptions> optionsAccessor,
            IAmazonS3 amazonS3,
            IFileMetaRepository fileMetaRepository)
        {
            _options = optionsAccessor.Value;
            _amazonS3 = amazonS3 ?? throw new ArgumentNullException(nameof(amazonS3));
            _fileMetaRepository = fileMetaRepository ?? throw new ArgumentNullException(nameof(fileMetaRepository));
        }

        public async Task DeleteFilesAsync(IEnumerable<FileMeta> files)
        {
            await _fileMetaRepository
                .DeleteFileMetaByIdsAsync(files.Select(file => file.Id!))
                .ConfigureAwait(false);

            var request = new DeleteObjectsRequest
            {
                BucketName = _options.Bucket,
                Objects = files.Select(file => new KeyVersion
                {
                    Key = file.FileName
                }).ToList()
            };

            await _amazonS3.DeleteObjectsAsync(request).ConfigureAwait(false);
        }

        public IAsyncEnumerable<FileMeta> FindExpiredFilesAsync(string prefix, DateTimeOffset expireTime)
            => _fileMetaRepository.ListExpiredFileMetasAsync(prefix, expireTime);

        public IAsyncEnumerable<FileMeta> ListPathAsync(string prefix)
            => _fileMetaRepository.ListFileMetasAsync(prefix);

        public Task<Stream> OpenFileStreamAsync(FileMeta meta)
        {
            return _amazonS3.GetObjectStreamAsync(_options.Bucket, meta.FileName, null);
        }

        public async Task<FileMeta> SaveAsync(string filePath, string contentType, long contentLength, Stream fileStream)
        {
            var savePath = filePath.TrimStart('/');

            var request = new PutObjectRequest
            {
                BucketName = _options.Bucket,
                Key = savePath,
                InputStream = fileStream
            };

            var response = await _amazonS3.PutObjectAsync(request);

            if (response.HttpStatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException(
                    $"[FileService] Couldn't upload file {filePath}.");

            var file = new FileMeta
            {
                FileName = savePath,
                ContentType = contentType,
                FileLength = contentLength,
                CreatedOn = DateTimeOffset.Now
            };

            var id = await _fileMetaRepository.AddFileMetaAsync(file).ConfigureAwait(false);

            file.Id = id;
            file.ExternalUrl = $"{_options.ServiceUrl}/{_options.Bucket}/{savePath}";

            return file;
        }
    }
}
