using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public sealed class RedisCoreConnection : BaseRedisConnection
    {
        public RedisCoreConnection(IRedisConfig config, IServiceLog<BaseRedisConnection> serviceLog)
            : base(config.RedisConfigurationString, serviceLog)
        {
        }
    }
}