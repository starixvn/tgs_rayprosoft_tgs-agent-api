using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Redis
{
    public sealed class RedisCoreConnection : BaseRedisConnection
    {
        public RedisCoreConnection(IRedisConfig config, RequestAnalyzer osLog)
            : base(config.RedisConfigurationString, osLog)
        {
        }
    }
}