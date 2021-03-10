using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.ValidateCode;

namespace TGS.Repositories.ValidateCode
{
    class RedisValidateCodeStore : IValidateCodeStore
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
            var value = await _redisDatabase
                .StringGetAsync($"{_KeyPrefix}:{token}:{validateCode}")
                .ConfigureAwait(false);

            return (string?)value;
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
