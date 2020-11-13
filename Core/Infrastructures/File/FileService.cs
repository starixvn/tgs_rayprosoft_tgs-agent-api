using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Utility;

namespace TGS.Core.Infrastructures.File
{
    public interface IFileService
    {
        public Task<string> UploadFileAsync(IFormFile formFile);
        public Task<string> GetUrl(string key);
    }

    public class FileService : IFileService
    {
        private static string _url;
        private readonly IAmazonS3 _amazonS3;
        private readonly IOptions<AwsSettings> _awsSettings;
        private readonly IOptions<FileSetting> _fileSetting;


        public FileService(IOptions<FileSetting> fileSetting, IOptions<AwsSettings> awsSettings, IAmazonS3 amazonS3)
        {
            _fileSetting = fileSetting;
            _url = _fileSetting.Value.Url;
            _amazonS3 = amazonS3;
            _awsSettings = awsSettings;
        }

        Task<string> IFileService.GetUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _awsSettings.Value.S3.Bucket,
                Key = key,
                Protocol = Protocol.HTTP,
                Expires = DateTime.UtcNow.AddDays(90)
            };
            var url = _amazonS3.GetPreSignedURL(request);
            return Task.FromResult(url);
        }

        async Task<string> IFileService.UploadFileAsync(IFormFile formFile)
        {
            var key = StringGenerator.GenerateAlphanumericString(32);
            var request = new PutObjectRequest
            {
                BucketName = _awsSettings.Value.S3.Bucket,
                Key = key,
                InputStream = formFile.OpenReadStream()
            };
            var response = await _amazonS3.PutObjectAsync(request);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"[FileService] Couldn't upload file {formFile.FileName} {formFile.ContentType} {formFile.Length}");
            }

            return $"{_awsSettings.Value.S3.ServiceUrl}/{_awsSettings.Value.S3.Bucket}/{key}";
        }
    }
}