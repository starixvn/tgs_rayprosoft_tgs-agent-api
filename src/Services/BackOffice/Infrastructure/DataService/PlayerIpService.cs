using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IPlayerIpService
    {
        IQueryable<PlayerLoginRecord> GetPlayerIpOperationQueries(Guid brandId);
        IQueryable<PlayerLoginRecord> GetPlayerIpOperationByIpQueries(Guid brandId, string ipAddress);
        IQueryable<PlayerLoginRecord> GetPlayerIpOperationByUsernameQueries(Guid brandId, string username);
    }
    public class PlayerIpService : IPlayerIpService
    {
        private readonly ITgsRepository _tgsRepository;
        public PlayerIpService(
            ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public IQueryable<PlayerLoginRecord> GetPlayerIpOperationQueries(Guid brandId)
            => _tgsRepository.PlayerLoginRecords.AsQueryable()
            .Where(x => x.BrandId == brandId);

        public IQueryable<PlayerLoginRecord> GetPlayerIpOperationByIpQueries(Guid brandId, string ipAddress)
            => _tgsRepository.PlayerLoginRecords.AsQueryable()
            .Where(x => x.BrandId == brandId)
            .Where(x => x.IpAddress == ipAddress);

        public IQueryable<PlayerLoginRecord> GetPlayerIpOperationByUsernameQueries(Guid brandId, string username)
            => _tgsRepository.PlayerLoginRecords.AsQueryable()
            .Where(x => x.BrandId == brandId)
            .Where(x => x.Player.Username == username);

    }
}
