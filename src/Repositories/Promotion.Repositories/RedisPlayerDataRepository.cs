using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.Promotion;

namespace TGS.Promotion.Repositories
{
    internal class RedisPlayerDataRepository : IPlayerDataRepository
    {
        private static readonly string _KeyPrefix = "TGS:Promotion";
        private readonly IDatabase _redisDatabase;
        private readonly IRedisRepository _redisRepository;

        public RedisPlayerDataRepository(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository ?? throw new ArgumentNullException(nameof(redisRepository));
            _redisDatabase = _redisRepository.GetDb();
        }

        public async ValueTask<bool> CheckPlayerFirstDepositingAsync(string playerCode)
        {
            var dataKey = $"{_KeyPrefix}:{playerCode}";

            return await _redisDatabase
                .KeyExistsAsync(dataKey)
                .ConfigureAwait(false);
        }

        public async ValueTask<bool> CheckPlayerIsDailyFirstDepositAsync(string playerCode)
        {
            var dataKey = $"{_KeyPrefix}:{DateTimeOffset.Now:yyyyMMdd}:{playerCode}";

            return !await _redisDatabase
                .KeyExistsAsync(dataKey)
                .ConfigureAwait(false);
        }

        public async ValueTask RemovePlayerDailyDepositFlagAsync(string playerCode)
        {
            var currentDate = DateTimeOffset.Now;
            var dataKey = $"{_KeyPrefix}:{currentDate:yyyyMMdd}:{playerCode}";

            var count = await _redisDatabase
                .StringDecrementAsync(dataKey)
                .ConfigureAwait(false);

            if (count <= 0)
                await _redisDatabase
                    .KeyDeleteAsync(dataKey, CommandFlags.FireAndForget)
                    .ConfigureAwait(false);
        }

        public async ValueTask RemovePlayerFirstDepositFlagAsync(string playerCode)
        {
            var dataKey = $"{_KeyPrefix}:{playerCode}";

            var count = await _redisDatabase
                .StringDecrementAsync(dataKey)
                .ConfigureAwait(false);

            if (count <= 0)
                await _redisDatabase
                    .KeyDeleteAsync(dataKey, CommandFlags.FireAndForget)
                    .ConfigureAwait(false);
        }

        public async ValueTask SetPlayerDailyDepositFlagAsync(string playerCode)
        {
            var currentDate = DateTimeOffset.Now;
            var key = $"{_KeyPrefix}:{currentDate:yyyyMMdd}:{playerCode}";

            await _redisDatabase
                .StringSetAsync(
                    $"{_KeyPrefix}:{currentDate:yyyyMMdd}:{playerCode}",
                    "0",
                    TimeSpan.FromDays(1D),
                    when: When.NotExists,
                    flags: CommandFlags.FireAndForget)
                .ConfigureAwait(false);

            await _redisDatabase
                .StringIncrementAsync(key, flags: CommandFlags.FireAndForget)
                .ConfigureAwait(false);
        }

        public async ValueTask SetPlayerFirstDepositFlagAsync(string playerCode)
        {
            var dataKey = $"{_KeyPrefix}:{playerCode}";

            await _redisDatabase
                .StringIncrementAsync(dataKey, flags: CommandFlags.FireAndForget)
                .ConfigureAwait(false);
        }
    }
}
