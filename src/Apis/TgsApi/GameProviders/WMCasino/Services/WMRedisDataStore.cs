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

        public async ValueTask AddBetDetailRequestInfoAsync(
            string agentId,
            string roundNumber,
            BetDetailRequestInfo info)
        {
            var key = GetBetDetailRequestInfoKey(agentId, roundNumber);
            var value = JsonSerializer.Serialize(info, _jsonSerializerOptions);

            await _database.ListRightPushAsync(key, value).ConfigureAwait(false);
        }

        public async ValueTask AddBetResultsToQueueAsync(
            string batchId,
            IEnumerable<BetDetail> betDetails)
        {
            var key = GetBetResultKey(batchId);

            var batch = _database.CreateBatch();

            var tasks = betDetails.Select(detail =>
            {
                var value = JsonSerializer.Serialize(detail, _jsonSerializerOptions);
                return _database.ListRightPushAsync(key, value);
            }).Cast<Task>().ToArray();

            batch.Execute();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async ValueTask ApplyBetResultBatchesAsync(IEnumerable<string> batchIds)
        {
            var key = "WM:BetResult:BatchId";
            var batch = _database.CreateBatch();

            var tasks = batchIds
                .Select(id => _database.ListRightPushAsync(key, id))
                .Cast<Task>()
                .ToArray();

            batch.Execute();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async ValueTask<string> GetBatchIdAsync()
        {
            var key = "WM:BetResult:BatchId";
            var value = await _database.ListLeftPopAsync(key).ConfigureAwait(false);
            return value.HasValue ? value.ToString() : null;
        }

        public async ValueTask<IEnumerable<BetDetailRequestInfo>> GetBetDetailRequestInfosAsync(
            string agentId,
            string roundNumber)
        {
            var key = GetBetDetailRequestInfoKey(agentId, roundNumber);

            var value = await _database.ListRangeAsync(key).ConfigureAwait(false);

            return value.Length == 0 ?
                Array.Empty<BetDetailRequestInfo>() :
                value.Select(v => JsonSerializer.Deserialize<BetDetailRequestInfo>(v));
        }

        public async ValueTask<IEnumerable<BetDetail>> GetBetResultsByBatchIdAsync(string batchId)
        {
            var key = GetBetResultKey(batchId);

            var value = await _database.ListRangeAsync(key).ConfigureAwait(false);

            return value.Length == 0 ?
                Array.Empty<BetDetail>() :
                value.Select(v => JsonSerializer.Deserialize<BetDetail>(v));
        }

        public async ValueTask RemoveBetDetailRequestInfosAsync(string agentId, string roundNumber)
        {
            var key = GetBetDetailRequestInfoKey(agentId, roundNumber);

            await _database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public async ValueTask RemoveBetResultsByBatchIdAsync(string batchId)
        {
            var key = GetBetResultKey(batchId);
            await _database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        private static string GetBetDetailRequestInfoKey(
            string agentId,
            string roundNumber)
            => $"WM.BetDetailRequestInfo:{agentId}:{roundNumber}";

        private static string GetBetResultKey(string batchId)
            => $"WM:BetResult:{batchId}";
    }
}
