using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using TGS.Apis.FileService.DataService.Model;
using TGS.Core.Infrastructures.Nats.Model;
using TGS.Core.Infrastructures.Utility;
using TGS.FileService;
using TGS.FileService.Repositories;

namespace TGS.Apis.FileService.DataService.Services
{
    public sealed class UploadService : IUploadService
    {
        private readonly IMongoContext<StationLetterFile> _stationLetterRepository;
        private readonly IFileManager _fileManager;

        public UploadService(
            IMongoContext<StationLetterFile> stationLetterRepository,
            IFileManager fileManager)
        {
            _stationLetterRepository = stationLetterRepository ?? throw new ArgumentNullException(nameof(stationLetterRepository));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public async Task<string> UploadFileS3Async(IFormFile formFile)
        {
            var randomFileName =
                $"{StringGenerator.GenerateAlphanumericString(32)}{Path.GetExtension(formFile.FileName)}";

            return (await _fileManager
                .SaveAsync(randomFileName, formFile.ContentType, formFile.Length, formFile.OpenReadStream())
                .ConfigureAwait(false)).ExternalUrl;
        }

        public async Task AddStationLetterFileAsync(ObjectFileEventModel model)
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

            await CleanFilesAsync();
        }

        public ValueTask<string[]> GetObjectsAsync(string prefix)
            => _fileManager.ListPathAsync(prefix)
                .Select(meta => meta.FileName)
                .ToArrayAsync();

        private async Task CleanFilesAsync()
        {
            var stationLetterFileNames =
                _stationLetterRepository.GetQueries()
                    .SelectMany(l => l.Files)
                    .Select(f => f);

            var files = _fileManager.FindExpiredFilesAsync("/", DateTimeOffset.Now.AddHours(-24))
                .Where(meta => !stationLetterFileNames.Contains(meta.FileName));

            if (!await files.AnyAsync().ConfigureAwait(false))
                return;

            await _fileManager.DeleteFilesAsync(files.ToEnumerable());
        }
    }
}
