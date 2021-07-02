using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Aws;
using TGS.Core.Infrastructures.Utility;
using TGS.FileService;

namespace TGS.Apis.FileService.DataService.Services
{
    public sealed class UploadService : IUploadService
    {
        private readonly IFileManager<IPublicAmazonS3> _fileManager;

        public UploadService(
            IFileManager<IPublicAmazonS3> fileManager)
        {
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public async Task<FileMeta> UploadFileS3Async(string prefix, IFormFile formFile)
        {
            var randomFileName =
                $"{StringGenerator.GenerateAlphanumericString(32)}{Path.GetExtension(formFile.FileName)}";

            return await _fileManager
                .SaveAsync($"{prefix}/{randomFileName}", formFile.ContentType, formFile.Length,
                    formFile.OpenReadStream())
                .ConfigureAwait(false);
        }

        public ValueTask<string[]> GetObjectsAsync(string prefix)
            => _fileManager.ListPathAsync(prefix)
                .Select(meta => meta.FileName)
                .ToArrayAsync();

        public async Task CleanFilesAsync(string prefix, string[] names)
        {
            var files = await _fileManager.FindExpiredFilesAsync($"/{prefix}/", DateTimeOffset.Now.AddHours(-24))
                .Where(meta => !names.Contains(meta.FileName))
                .ToArrayAsync()
                .ConfigureAwait(false);

            if (files.Length == 0)
            {
                return;
            }

            await _fileManager.DeleteFilesAsync(files);
        }
    }
}