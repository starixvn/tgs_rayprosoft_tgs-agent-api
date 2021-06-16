using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TGS.Apis.FileService.DataService.Model;
using TGS.FileService.Repositories;

namespace TGS.Apis.FileService.DataService.Services
{
    public sealed class PromotionService : IPromotionService
    {
        private readonly IMongoContext<PromotionFile> _stationLetterRepository;

        public PromotionService(
            IMongoContext<PromotionFile> stationLetterRepository)
        {
            _stationLetterRepository = stationLetterRepository ??
                throw new ArgumentNullException(nameof(stationLetterRepository));
        }

        public async Task AddPromotionFileAsync(string id, string[] fileNames)
        {
            var filter = Builders<PromotionFile>.Filter.Eq(x => x.PromotionId, id);
            var stationLetterFile = await _stationLetterRepository.GetByFilterAsync(filter);
            if (stationLetterFile != null)
            {
                stationLetterFile.Files = fileNames;
                stationLetterFile.UpdatedOn = DateTimeOffset.Now;

                await _stationLetterRepository.UpdateAsync(stationLetterFile);
            }
            else
            {
                stationLetterFile = new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    PromotionId = id,
                    Files = fileNames,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                };

                await _stationLetterRepository.InsertAsync(stationLetterFile);
            }
        }
    }
}