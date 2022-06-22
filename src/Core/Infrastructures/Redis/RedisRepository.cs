using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis;

public sealed class RedisRepository : IRedisRepository
{
    private readonly IRedisConnectionFactory _connectionFactory;

    public RedisRepository(IRedisConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    void IRedisRepository.DeleteKey(string key)
    {
        var db = GetDatabase();
        db.KeyDelete(key);
    }

    Task IRedisRepository.DeleteKeyAsync(string key)
    {
        var db = GetDatabase();
        return db.KeyDeleteAsync(key);
    }

    string IRedisRepository.GetConnectionStatus()
    {
        var connection = GetConnection();
        return connection.GetStatus();
    }

    IDatabase IRedisRepository.GetDb()
    {
        return GetDatabase();
    }

    private IConnectionMultiplexer GetConnection()
    {
        return _connectionFactory.GetConnection();
    }

    private IDatabase GetDatabase()
    {
        var connection = GetConnection();
        return connection.GetDatabase();
    }
}
