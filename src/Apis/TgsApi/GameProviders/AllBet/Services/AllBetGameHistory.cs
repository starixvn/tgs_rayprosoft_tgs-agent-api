using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public class AllBetGameHistory : BaseBetHistory
    {
        public AllBetGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations)
            : base(roundQueries, gameProviderOperations, localizationOperations)
        {
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider,
            CachedGame cachedGame, string language)
            => throw new NotImplementedException();
    }
}