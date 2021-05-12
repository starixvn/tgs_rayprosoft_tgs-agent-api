using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.GameOperations.Data;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.VR.Outgoing;
using TGS.Core.Infrastructures.Extensions;
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

        protected async override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round, CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var gameRecord = JsonSerializer.Deserialize<VRGameRecord>(round.RoundDetail.AdditionalData);

            if (gameRecord == null)
            {
                return null;
            }


            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                Data = new BaseData[]
               {
                    new JsonData {Value = JsonSerializer.Serialize(gameRecord.PrizeDetail)}
               }
            };
        }
    }
}
