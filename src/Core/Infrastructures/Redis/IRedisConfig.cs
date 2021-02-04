namespace TGS.Core.Infrastructures.Redis
{
    public interface IRedisConfig
    {
        string RedisConfigurationString { get; }
    }
}