using Microsoft.Extensions.Logging;

namespace TGS.Infrastructures.Redis
{
    public sealed class RedisCoreConnection : BaseRedisConnection
    {
        public RedisCoreConnection(IRedisConfig config, ILogger<BaseRedisConnection> logger)
            : base(config.RedisConfigurationString, logger)
        {
        }
    }
}