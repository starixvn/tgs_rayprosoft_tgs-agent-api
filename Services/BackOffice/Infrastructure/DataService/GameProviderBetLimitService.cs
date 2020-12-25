using System;
using System.Linq;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IGameProviderBetLimitService
    {
        IQueryable<GameProviderBetLimit> GetGameProviderBetLimitsAsync(
            Func<IQueryable<GameProviderBetLimit>, IQueryable<GameProviderBetLimit>> preQuery = null,
            bool tracking = false);
    }

    public class GameProviderBetLimitService : BaseService, IGameProviderBetLimitService
    {
        public GameProviderBetLimitService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
        }

        IQueryable<GameProviderBetLimit> IGameProviderBetLimitService.GetGameProviderBetLimitsAsync(
            Func<IQueryable<GameProviderBetLimit>, IQueryable<GameProviderBetLimit>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}