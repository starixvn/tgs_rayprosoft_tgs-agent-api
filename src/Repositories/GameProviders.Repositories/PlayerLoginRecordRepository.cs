using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories.Models;

namespace TGS.GameProviders.Repositories
{
    internal class PlayerLoginRecordRepository : IPlayerLoginRecordRepository
    {
        private readonly ITgsRepository _repository;

        public PlayerLoginRecordRepository(ITgsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<PlayerLoginRecordModel> GetAsync(Guid loginRecordId)
            => _repository.PlayerLoginRecords
            .Where(x => x.Id == loginRecordId)
            .Select(x => new PlayerLoginRecordModel()
            {
                Id = x.Id,
                PlayerCode = x.PlayerCode,
                BrandId = x.BrandId,
                Status = x.Status,
                PlatformType = x.PlatformType,
                IpAddress = x.IpAddress,
                CreatedOn = x.CreatedOn,
                ExpiredOn = x.ExpiredOn
            }).FirstOrDefaultAsync();

        public Task<PlayerLoginRecordModel[]> QueryEnabledAsync(string playerCode)
        {
            var expiredOn = DateTimeOffset.UtcNow.AddDays(-1);
            return _repository.PlayerLoginRecords
             .Where(x => x.PlayerCode == playerCode &&
                     x.Status == LoginStatus.Enabled &&
                     x.ExpiredOn > expiredOn)
             .OrderByDescending(x => x.ExpiredOn)
             .Select(x => new PlayerLoginRecordModel()
             {
                 Id = x.Id,
                 PlayerCode = x.PlayerCode,
                 BrandId = x.BrandId,
                 Status = x.Status,
                 PlatformType = x.PlatformType,
                 IpAddress = x.IpAddress,
                 CreatedOn = x.CreatedOn,
                 ExpiredOn = x.ExpiredOn
             }).ToArrayAsync();
        }
    }
}
