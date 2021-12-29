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

        public async ValueTask AddBetResultsToQueueAsync(IEnumerable<BetDetail> betDetails)
        {
            var dataKey = "WM:BetResult";
            var batch = _database.CreateBatch();

            var tasks = betDetails.Select(detail =>
            {
                var value = JsonSerializer.Serialize(detail, _jsonSerializerOptions);
                return _database.ListRightPushAsync(dataKey, value);
            }).Cast<Task>().ToArray();

            batch.Execute();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async ValueTask<BetDetail> GetBetResultFromQueueAsync()
        {
            var dataKey = "WM:BetResult";

            var value = await _database.ListLeftPopAsync(dataKey).ConfigureAwait(false);

            return value.HasValue ?
                JsonSerializer.Deserialize<BetDetail>(value) :
                null;
        }
    }
}
