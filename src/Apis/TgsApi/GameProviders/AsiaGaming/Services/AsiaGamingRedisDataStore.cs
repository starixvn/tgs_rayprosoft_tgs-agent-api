using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Data;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public class AsiaGamingRedisDataStore : IAsiaGamingDataStore
    {
        private readonly IDatabase _database;

        public AsiaGamingRedisDataStore(
            IDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async ValueTask<bool> ExistBetResponseTransactionIdAsync(string transactionId)
        {
            var dataKey = GetBetResponseTransactionIdDataKey(transactionId);

            var value = await _database.StringGetAsync(dataKey)
                .ConfigureAwait(false);

            return value.HasValue && (bool)value;
        }

        public async ValueTask<IEnumerable<BetInfo>> GetBetInfosAsync(string roundId)
        {
            var dataKey = GetBetInfoDataKey(roundId);

            var values = await _database.HashGetAllAsync(dataKey).ConfigureAwait(false);

            return values.Select(v => new BetInfo(v.Name, (bool)v.Value));
        }

        public async ValueTask RemoveBetInfosAsync(string roundId)
        {
            var dataKey = GetBetInfoDataKey(roundId);

            _ = await _database.KeyDeleteAsync(dataKey).ConfigureAwait(false);
        }

        public async ValueTask RemoveBetResponseTransactionIdAsync(string transactionId)
        {
            var dataKey = GetBetResponseTransactionIdDataKey(transactionId);

            _ = await _database.KeyDeleteAsync(dataKey).ConfigureAwait(false);
        }

        public async ValueTask StoreBetInfoAsync(string roundId, string transactionId, bool settled)
        {
            var dataKey = GetBetInfoDataKey(roundId);

            _ = await _database
                .HashSetAsync(dataKey, transactionId, settled)
                .ConfigureAwait(false);

            await _database.KeyExpireAsync(dataKey, TimeSpan.FromDays(7)).ConfigureAwait(false);
        }

        public async ValueTask<bool> StoreBetResponseTransactionIdAsync(string transactionId)
        {
            var dataKey = GetBetResponseTransactionIdDataKey(transactionId);

            return await _database.StringSetAsync(dataKey, true, when: When.NotExists)
                .ConfigureAwait(false);
        }

        private string GetBetInfoDataKey(string roundId)
            => $"AG:BetInfo:{roundId}";

        private string GetBetResponseTransactionIdDataKey(string transactionId)
            => $"AG:BetResponse:{transactionId}";
    }
}
