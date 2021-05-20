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

        public async ValueTask<bool> CheckPlayerIsDailyFirstDepositAsync(string playerCode)
        {
            var dataKey = $"{_KeyPrefix}:{DateTimeOffset.Now:yyyyMMdd}:{playerCode}";

            return await _redisDatabase
                .KeyExistsAsync(dataKey)
                .ConfigureAwait(false);
        }

        public async ValueTask SetPlayerDepositAsync(string playerCode)
        {
            var currentDate = DateTimeOffset.Now;

            await _redisDatabase
                .StringSetAsync(
                    $"{_KeyPrefix}:{DateTimeOffset.Now:yyyyMMdd}:{playerCode}",
                    "1",
                    TimeSpan.FromDays(1D),
                    when: When.NotExists,
                    flags: CommandFlags.FireAndForget)
                .ConfigureAwait(false);
        }
    }
}
