using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Redis
{
    public sealed class RedisSignalRConnection : BaseRedisConnection
    {
        public RedisSignalRConnection(IRedisConfig config, IOsLog osLog)
            : base(config.SignalRRedisConnectionString, osLog)
        {
        }
    }
}