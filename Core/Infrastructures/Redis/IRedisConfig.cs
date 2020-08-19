namespace TGS.Infrastructures.Redis
{
    public interface IRedisConfig
    {
        string RedisConfigurationString { get; }
        string SignalRRedisConnectionString { get; }
    }
}