using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public class RedisPublisher : IRedisPublisher
    {
        private readonly IRedisConnectionFactory _connectionFactory;

        public RedisPublisher(
            IRedisConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        Task<long> IRedisPublisher.PublishAsync<T>(T message)
        {
            var connection = GetConnection();

            var messageValue = JsonSerializer.Serialize(message);

            return connection.GetSubscriber().PublishAsync($"{typeof(T).FullName}", messageValue);
        }

        private IConnectionMultiplexer GetConnection()
        {
            return _connectionFactory.GetConnection();
        }
    }
}
