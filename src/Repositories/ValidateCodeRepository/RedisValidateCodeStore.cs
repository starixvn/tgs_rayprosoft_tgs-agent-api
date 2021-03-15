using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.ValidateCode;

namespace TGS.Repositories.ValidateCode
{
    internal class RedisValidateCodeStore : IValidateCodeStore
    {
        private static readonly string _KeyPrefix = "TGS:ValidateCodes";
        private static readonly TimeSpan _Expire = TimeSpan.FromMinutes(3);
        private readonly IRedisRepository _redisRepository;
        private readonly IDatabase _redisDatabase;

        public RedisValidateCodeStore(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository ?? throw new ArgumentNullException(nameof(redisRepository));
            _redisDatabase = _redisRepository.GetDb();
        }

        public async ValueTask<string?> GetDataAsync(string validateCode, string token)
        {
            var dataKey = $"{_KeyPrefix}:{token}:{validateCode}";
            var value = await _redisDatabase
                .StringGetAsync(dataKey)
                .ConfigureAwait(false);

            await _redisDatabase
                .KeyDeleteAsync(dataKey, flags: CommandFlags.FireAndForget)
                .ConfigureAwait(false);

            return value;
        }

        public async ValueTask<bool> SaveDataAsync(string validateCode, string token, string content)
        {
            return await _redisDatabase.StringSetAsync(
                $"{_KeyPrefix}:{token}:{validateCode}",
                content,
                _Expire,
                when: When.NotExists).ConfigureAwait(false);
        }
    }
}
