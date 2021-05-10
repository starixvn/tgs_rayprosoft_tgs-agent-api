using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.VR.Services
{
    public sealed class VRGameHistory : BaseBetHistory
    {
        private readonly IGameRoundsProcessor _gameRoundsProcessor;

        public VRGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IGameRoundsProcessor gameRoundsProcessor
            ) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round, CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            throw new NotImplementedException();
        }
    }
}
