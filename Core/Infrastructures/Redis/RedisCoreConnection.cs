using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Redis
{
    public sealed class RedisCoreConnection : BaseRedisConnection
    {
        public RedisCoreConnection(IRedisConfig config, IServiceLog<BaseRedisConnection> serviceLog)
            : base(config.RedisConfigurationString, serviceLog)
        {
        }
    }
}