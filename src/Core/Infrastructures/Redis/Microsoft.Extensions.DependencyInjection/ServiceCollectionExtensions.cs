using System;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Infrastructures.Redis.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Dependency Injection of Redis, need to add the following dependencies beforehand:
        /// 1. AddJson() in Infrastructures.Json
        /// 2. AddTgsLog() in Infrastructures.Log
        /// </summary>
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();

            services.AddSingleton<IRedisRepository, RedisRepository>();
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));

            return services;
        }

        /// <summary>
        /// Dependency Injection of Redis, need to add the following dependencies beforehand:
        /// 1. AddJson() in Infrastructures.Json
        /// 2. AddTgsLog() in Infrastructures.Log
        /// </summary>
        public static RedisConfiguration AddRedisServices(this IServiceCollection services)
        {
            services.AddSingleton<IJsonSerializationProvider, JsonSerializationProvider>();

            services.AddSingleton<IRedisRepository, RedisRepository>();
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));

            var configuration = new RedisConfiguration(services);

            return configuration;
        }
    }
}
