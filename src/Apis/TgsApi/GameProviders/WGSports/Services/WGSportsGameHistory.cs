using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.WGSports.Commons;

namespace TGS.Apis.TgsApi.GameProviders.WGSports.Services
{
    public sealed class WGSportsGameHistory : BaseBetHistory
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public WGSportsGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            IJsonSerializationProvider jsonSerializationProvider
        ) : base(roundQueries, gameProviderOperations)
        {
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var gameRecord = _jsonSerializationProvider.DeserializeFromString<WGSportsBetInfo>(round.RoundDetail.AdditionalData);

            if (gameRecord == null)
            {
                return ValueTask.FromResult<RoundResult>(null);
            }

            return ValueTask.FromResult(new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData {Value = _jsonSerializationProvider.SerializeToString(gameRecord)}
                }
            });
        }
    }
}
