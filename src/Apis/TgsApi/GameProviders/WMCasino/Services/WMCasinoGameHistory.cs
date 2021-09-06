using System;
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

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMCasinoGameHistory : BaseBetHistory
    {
        private readonly IJsonSerializationProvider _json;

        public WMCasinoGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            IJsonSerializationProvider json)
            : base(roundQueries, gameProviderOperations)
        {
            _json = json;
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
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

        private string GetGameHistory(string roundDetail)
        {
            if (string.IsNullOrWhiteSpace(roundDetail))
            {
                return string.Empty;
            }

            var results = roundDetail.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            if (results.Length == 0)
            {
                return string.Empty;
            }

            if (results.Length == 1)
            {
                return results[0];
            }

            var items = results.Select(r => r.Split(":", StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(r => r[0], r => r[1]);

            return items.Count == 0 ? string.Empty : _json.SerializeToString(items);
        }
    }
}