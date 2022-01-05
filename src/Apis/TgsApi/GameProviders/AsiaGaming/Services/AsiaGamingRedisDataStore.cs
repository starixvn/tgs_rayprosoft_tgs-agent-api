using System;
using System.Threading.Tasks;
using StackExchange.Redis;

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

        public async ValueTask RemoveBetResponseTransactionIdAsync(string transactionId)
        {
            var dataKey = GetBetResponseTransactionIdDataKey(transactionId);

            _ = await _database.KeyDeleteAsync(dataKey).ConfigureAwait(false);
        }

        public async ValueTask<bool> StoreBetResponseTransactionIdAsync(string transactionId)
        {
            var dataKey = GetBetResponseTransactionIdDataKey(transactionId);

            return await _database.StringSetAsync(dataKey, true, when: When.NotExists)
                .ConfigureAwait(false);
        }

        private string GetBetResponseTransactionIdDataKey(string transactionId)
            => $"AG:BetResponse:{transactionId}";
    }
}
