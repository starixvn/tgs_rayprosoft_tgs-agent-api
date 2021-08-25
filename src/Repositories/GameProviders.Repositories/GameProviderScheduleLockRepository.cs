using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;

namespace TGS.GameProviders.Repositories
{
    public sealed class GameProviderScheduleLockRepository : IGameProviderScheduleLockRepository
    {
        private readonly IRedisConnection _connectionFactory;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly string _prefix = "GameProviderScheduleLock";

        public GameProviderScheduleLockRepository(
            IRedisConnection connectionFactory,
            IJsonSerializationProvider jsonSerializationProvider
            )
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        async ValueTask<bool> IGameProviderScheduleLockRepository.IsLockAsync(string gameProviderCode, string key)
        {
            var connection = _connectionFactory.GetConnection();
            var db = connection.GetDatabase();
            var value = await db.StringGetAsync(Format(gameProviderCode, key)).ConfigureAwait(false);
            return !string.IsNullOrEmpty(value);
        }

        async ValueTask<bool> IGameProviderScheduleLockRepository.RemoveAsync(string gameProviderCode, string key)
        {
            var connection = _connectionFactory.GetConnection();
            var db = connection.GetDatabase();
            return await db.KeyDeleteAsync(Format(gameProviderCode, key)).ConfigureAwait(false);
        }

        async ValueTask<bool> IGameProviderScheduleLockRepository.SetAsync(string gameProviderCode, string key, GameProviderScheduleLockData date)
        {
            var connection = _connectionFactory.GetConnection();
            var db = connection.GetDatabase();
            return await db.StringSetAsync(Format(gameProviderCode, key), _jsonSerializationProvider.SerializeToString(date)).ConfigureAwait(false);
        }

        async ValueTask<GameProviderScheduleLockData> IGameProviderScheduleLockRepository.GetAsync(string gameProviderCode, string key)
        {
            var connection = _connectionFactory.GetConnection();
            var db = connection.GetDatabase();
            var value = await db.StringGetAsync(Format(gameProviderCode, key)).ConfigureAwait(false);
            return _jsonSerializationProvider.DeserializeFromString<GameProviderScheduleLockData>(value);
        }

        async ValueTask IGameProviderScheduleLockRepository.RemoveGameProviderCodeAsync(string gameProviderCode)
        {
            var connection = _connectionFactory.GetConnection();
            var server = connection.GetServer(_connectionFactory.GetConnection().GetEndPoints().First());
            var db = connection.GetDatabase();
            var keys = server.Keys(database: db.Database, pattern: $"{_prefix}:{gameProviderCode}:*", pageSize: int.MaxValue);
            var tasks = new List<Task>();
            foreach (var key in keys)
            {
                Task task = Task.Run(()
                    => db.KeyDeleteAsync(key));
                tasks.Add(task);
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private string Format(string gameProviderCode, string key)
        {
            return string.Format("{0}:{1}:{2}", _prefix, gameProviderCode, key);
        }

    }
}
