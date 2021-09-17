using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public class Card365GameHistory : BaseBetHistory
    {
        public Card365GameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations)
            : base(roundQueries, gameProviderOperations)
        {
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode,
            Round round, CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var value = GetGameHistory(round.RoundDetail?.AdditionalData);

            return ValueTask.FromResult(new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData { Value = value }
                }
            });
        }

        private static string GetGameHistory(string roundDetail)
        {
            if (string.IsNullOrWhiteSpace(roundDetail))
            {
                return string.Empty;
            }

            var items = JsonSerializer.Deserialize<Dictionary<string, object>>(roundDetail);
            if (items == null)
            {
                return string.Empty;
            }

            return items.TryGetValue("Tiles", out var tiles) ? tiles.ToString() : string.Empty;
        }
    }
}