using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{
    internal class Help2PayRedisRepository : IHelp2PayRedisRepository
    {
        private static readonly TimeSpan _Expire = TimeSpan.FromHours(1);
        private static readonly string _KeyPrefix = "Help2Pay:DepositHtml";

        private readonly IDatabase _redisDatabase;

        public Help2PayRedisRepository(RedisConnectionFactory connectionFactory)
        {
            if (connectionFactory is null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _redisDatabase = connectionFactory.Database;
        }

        public async ValueTask<string?> GetDepositHtmlAsync(Guid identityId)
        {
            var dataKey = $"{_KeyPrefix}:{identityId}";

            return await _redisDatabase.StringGetAsync(dataKey)
                .ConfigureAwait(false);
        }

        public async ValueTask<bool> StoreDepositHtmlAsync(Guid identityId, string html)
        {
            var dataKey = $"{_KeyPrefix}:{identityId}";

            return await _redisDatabase.StringSetAsync(dataKey, html, _Expire, When.NotExists)
                .ConfigureAwait(false);
        }
    }
}
