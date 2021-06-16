using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.FileService.DataService.Model;
using TGS.Core.Infrastructures.Utility;
using TGS.FileService;
using TGS.FileService.Repositories;

namespace TGS.Apis.FileService.DataService.Services
{
    public sealed class UploadService : IUploadService
    {
        private readonly IFileManager _fileManager;
        private readonly IMongoContext<StationLetterFile> _stationLetterRepository;

        public UploadService(
            IMongoContext<StationLetterFile> stationLetterRepository,
            IFileManager fileManager)
        {
            _stationLetterRepository = stationLetterRepository ??
                throw new ArgumentNullException(nameof(stationLetterRepository));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public async Task<string> UploadFileS3Async(string prefix, IFormFile formFile)
        {
            var randomFileName =
                $"{StringGenerator.GenerateAlphanumericString(32)}{Path.GetExtension(formFile.FileName)}";

            return (await _fileManager
                .SaveAsync($"{prefix}/{randomFileName}", formFile.ContentType, formFile.Length,
                    formFile.OpenReadStream())
                .ConfigureAwait(false)).ExternalUrl;
        }

        public ValueTask<string[]> GetObjectsAsync(string prefix)
            => _fileManager.ListPathAsync(prefix)
                .Select(meta => meta.FileName)
                .ToArrayAsync();

        public async Task CleanFilesAsync(string prefix)
        {
            var stationLetterFileNames =
                _stationLetterRepository.GetQueries()
                    .SelectMany(l => l.Files);

            var files = await _fileManager.FindExpiredFilesAsync($"/{prefix}/", DateTimeOffset.Now.AddHours(-24))
                .Where(meta => !stationLetterFileNames.Contains(meta.FileName))
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