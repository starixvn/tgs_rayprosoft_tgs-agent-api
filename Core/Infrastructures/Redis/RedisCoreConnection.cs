namespace TGS.Core.Infrastructures.Redis
{
    public sealed class RedisCoreConnection : BaseRedisConnection
    {
        public RedisCoreConnection(IRedisConfig config)
            : base(config.RedisConfigurationString)
        {
        }
    }
}