using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Infrastructures.Json;

namespace TGS.Infrastructures.Redis
{
    public class RedisSubscriber : IRedisSubscriber
    {
        private readonly IRedisConnection _connectionFactory;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public RedisSubscriber(
            IRedisConnection connectionFactory,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _connectionFactory = connectionFactory;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        Task IRedisSubscriber.SubscribeAsync<T>(Action<T> handler)
        {
            var connection = GetConnection();

            return connection.GetSubscriber().SubscribeAsync($"{typeof(T).FullName}", (channel, message) =>
            {
                handler(_jsonSerializationProvider.DeserializeFromString<T>(message));
            });
        }

        Task IRedisSubscriber.UnsubscribeAsync<T>()
        {
            var connection = GetConnection();
            return connection.GetSubscriber().UnsubscribeAsync($"{typeof(T).FullName}");
        }

        private IConnectionMultiplexer GetConnection()
        {
            return _connectionFactory.GetConnection();
        }
    }
}