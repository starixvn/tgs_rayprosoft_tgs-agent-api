using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Log;

namespace TGS.Core.Infrastructures.Redis
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();
            services.AddScoped<IRedisConfig, RedisConfig>();
            services.AddScoped<IRedisConnection, RedisCoreConnection>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IRedisCacheManager, RedisCacheManager>();
            services.AddScoped<IRedisLock, RedisLock>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));
            services.AddScoped<IRedisPublisher, RedisPublisher>();
            services.AddScoped<IRedisSubscriber, RedisSubscriber>();
            services.AddTransient(typeof(IServiceLog<>), typeof(ServiceLog<>));
            services.AddLogging();

            return services;
        }
    }
}