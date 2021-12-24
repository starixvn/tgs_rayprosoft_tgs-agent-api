using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.BetConstruct.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.TF.Services
{
    public class TFGameHistory : BaseBetHistory
    {
        public TFGameHistory(
           IRoundQueries roundQueries,
           IGameProviderOperations gameProviderOperations)
           : base(roundQueries, gameProviderOperations)
        {
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(
            string playerCode,
            Round round,
            CachedGameProvider cachedGameProvider,
            CachedGame cachedGame,
            string language)
        {
            var gameRecord = JsonSerializer.Deserialize<BCGameRecord[]>(round.RoundDetail.AdditionalData);

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
                    new JsonData
                    {
                        Value = JsonSerializer.Serialize(gameRecord)
                    }
                }
            });
        }
    }
}
