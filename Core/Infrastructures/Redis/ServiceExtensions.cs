using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;

namespace TGS.Core.Infrastructures.Redis
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Redis, need to add the following dependencies beforehead:
        /// 1. AddJson() in Infrastructures.Json
        /// 2. AddTgpLog() in Infrastructures.Log
        /// 3. AddLogging() in .net core
        /// </summary>
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();
            services.AddSingleton<IRedisConfig, RedisConfig>();
            services.AddSingleton<IRedisConnection, RedisCoreConnection>();
            services.AddSingleton<IRedisRepository, RedisRepository>();
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            services.AddSingleton<IRedisLock, RedisLock>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));
            services.AddSingleton<IRedisPublisher, RedisPublisher>();
            services.AddSingleton<IRedisSubscriber, RedisSubscriber>();

            return services;
        }
    }
}