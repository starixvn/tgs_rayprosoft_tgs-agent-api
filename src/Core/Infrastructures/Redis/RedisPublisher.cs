using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;

namespace TGS.Core.Infrastructures.Redis
{
    public class RedisPublisher : IRedisPublisher
    {
        private readonly IRedisConnection _connectionFactory;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public RedisPublisher(
            IRedisConnection connectionFactory,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _connectionFactory = connectionFactory;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        Task<long> IRedisPublisher.PublishAsync<T>(T message)
        {
            var connection = GetConnection();

            var messageValue = _jsonSerializationProvider.SerializeToString(message);

            return connection.GetSubscriber().PublishAsync($"{typeof(T).FullName}", messageValue);
        }

        private IConnectionMultiplexer GetConnection()
        {
            return _connectionFactory.GetConnection();
        }
    }
}