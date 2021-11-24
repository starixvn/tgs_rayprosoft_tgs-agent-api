using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TGS.Core.Infrastructures.Redis
{
    public class RedisSubscriber : IRedisSubscriber
    {
        private readonly IRedisConnectionFactory _connectionFactory;

        public RedisSubscriber(
            IRedisConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        Task IRedisSubscriber.SubscribeAsync<T>(Action<T> handler)
        {
            var connection = GetConnection();

            return connection.GetSubscriber().SubscribeAsync($"{typeof(T).FullName}", (channel, message) =>
            {
                handler(JsonSerializer.Deserialize<T>(message));
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
