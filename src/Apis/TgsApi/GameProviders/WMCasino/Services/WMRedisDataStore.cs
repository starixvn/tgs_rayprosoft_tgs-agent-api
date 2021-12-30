using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Models;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMRedisDataStore : IWMDataStore
    {
        private readonly IDatabase _database;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public WMRedisDataStore(
            IDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        public async ValueTask AddBetResultsToQueueAsync(
            string batchId,
            IEnumerable<BetDetail> betDetails)
        {
            var key = $"WM:BetResult:{batchId}";

            var batch = _database.CreateBatch();

            var tasks = betDetails.Select(detail =>
            {
                var value = JsonSerializer.Serialize(detail, _jsonSerializerOptions);
                return _database.ListRightPushAsync(key, value);
            }).Cast<Task>().ToArray();

            batch.Execute();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async ValueTask AddPlayerCodeByRoundNumberAsync(string roundNumber, string playerCode)
        {
            var key = $"WM:Round:{roundNumber}";

            await _database.ListRightPushAsync(key, playerCode).ConfigureAwait(false);
            await _database.KeyExpireAsync(key, TimeSpan.FromHours(2));
        }

        public async ValueTask ApplyBetResultBatchesAsync(string roundNumber)
        {
            var values = await GetPlayerCodesByRoundNumberAsync(roundNumber)
                .ConfigureAwait(false);

            var key = "WM:BetResult:BatchId";
            var batch = _database.CreateBatch();

            var tasks = values.Select(v =>
            {
                var batchId = $"{v}:{roundNumber}";
                return _database.ListRightPushAsync(key, batchId);
            }).Cast<Task>().ToArray();

            batch.Execute();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async ValueTask<string> GetBatchIdAsync()
        {
            var key = "WM:BetResult:BatchId";
            var value = await _database.ListLeftPopAsync(key).ConfigureAwait(false);
            return value.HasValue ? value.ToString() : null;
        }

        public async ValueTask<IEnumerable<BetDetail>> GetBetResultsByBatchIdAsync(string batchId)
        {
            var key = $"WM:BetResult:{batchId}";

            var value = await _database.ListRangeAsync(key).ConfigureAwait(false);

            return value.Length == 0 ?
                Array.Empty<BetDetail>() :
                value.Select(v => JsonSerializer.Deserialize<BetDetail>(v));
        }

        private async ValueTask<IEnumerable<string>> GetPlayerCodesByRoundNumberAsync(string roundNumber)
        {
            var key = $"WM:Round:{roundNumber}";
            var values = await _database.ListRangeAsync(key).ConfigureAwait(false);
            return values.ToStringArray().Distinct();
        }
    }
}
