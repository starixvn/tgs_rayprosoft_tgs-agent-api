using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.LY.Commons;

namespace TGS.Apis.TgsApi.GameProviders.LY.Services
{
    public sealed class LYGameHistory : BaseBetHistory
    {
        public LYGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations
        ) : base(roundQueries, gameProviderOperations)
        {
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(
            string playerCode,
            Round round,
            CachedGameProvider cachedGameProvider,
            CachedGame cachedGame,
            string language)
        {
            var gameRecord = JsonSerializer.Deserialize<LYGameRecord>(round.RoundDetail.AdditionalData, new JsonSerializerOptions
            {
                // 設定在做序列化/反序列化時，尊重名稱大小寫
                PropertyNameCaseInsensitive = true
            });

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
                    new JsonData {Value = gameRecord.CardValue}
                }
            });
        }
    }
}
