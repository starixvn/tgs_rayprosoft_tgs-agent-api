using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IGameProviderBetLimitService
    {
        IQueryable<GameProviderBetLimit> GetGameProviderBetLimitsAsync(
            Func<IQueryable<GameProviderBetLimit>, IQueryable<GameProviderBetLimit>> preQuery = null,
            bool tracking = false);

        Task<string[]> GetInUseBrandsAsync(Guid gameProviderBetLimitId);
    }

    public class GameProviderBetLimitService : BaseService, IGameProviderBetLimitService
    {
        private readonly ITgsRepository _repository;

        public GameProviderBetLimitService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
            _repository = tgsRepository;
        }

        IQueryable<GameProviderBetLimit> IGameProviderBetLimitService.GetGameProviderBetLimitsAsync(
            Func<IQueryable<GameProviderBetLimit>, IQueryable<GameProviderBetLimit>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task<string[]> IGameProviderBetLimitService.GetInUseBrandsAsync(Guid gameProviderBetLimitId)
        {
            return _repository.BrandGroupGameProviderBetLimits
                .Where(x => x.GameProviderBetLimitId == gameProviderBetLimitId)
                .Select(x => x.BrandGroupBetLimit.BrandGroup.Brand.Name).ToArrayAsync();
        }
    }
}