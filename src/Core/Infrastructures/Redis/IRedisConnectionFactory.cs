using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis;

public interface IRedisConnectionFactory
{
    public IConnectionMultiplexer GetConnection();
}
