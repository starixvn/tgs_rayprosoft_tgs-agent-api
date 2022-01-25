using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    internal class TCGCacheService : ITCGCacheService
    {
        private const string _key = "TCG:{0}:{1}";
        private readonly IDatabase _database;

        public TCGCacheService(IDatabase database)
        {
            _database = database ?? throw new System.ArgumentNullException(nameof(database));
        }

        public async ValueTask AddAsync(string userName, string refNo)
        {
            _ = await _database.StringSetAsync(
                    string.Format(_key, userName, refNo),
                    JsonSerializer.Serialize(new { LogOn = DateTime.UtcNow }),
                    TimeSpan.FromMinutes(30)).ConfigureAwait(false);
        }

        public Task<bool> ExistAsync(string userName, string refNo)
            => _database.KeyExistsAsync(string.Format(_key, userName, refNo));

        public async ValueTask RemoveAsync(string userName, string refNo)
        {
            _ = await _database.KeyDeleteAsync(
                string.Format(_key, userName, refNo)).ConfigureAwait(false);
        }
    }
}
