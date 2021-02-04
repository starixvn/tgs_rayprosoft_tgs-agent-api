using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public sealed class RedisRepository : IRedisRepository
    {
        private readonly IRedisConnection _connectionFactory;

        public RedisRepository(IRedisConnection connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        IDatabase IRedisRepository.GetDb()
        {
            return GetDatabase();
        }

        Task IRedisRepository.DeleteKeyAsync(string key)
        {
            var db = GetDatabase();
            return db.KeyDeleteAsync(key);
        }

        void IRedisRepository.DeleteKey(string key)
        {
            var db = GetDatabase();
            db.KeyDelete(key);
        }

        private IDatabase GetDatabase()
        {
            var connection = GetConnection();
            return connection.GetDatabase();
        }

        string IRedisRepository.GetConnectionStatus()
        {
            var connection = GetConnection();
            return connection.GetStatus();
        }

        private IConnectionMultiplexer GetConnection()
        {
            return _connectionFactory.GetConnection();
        }
    }
}