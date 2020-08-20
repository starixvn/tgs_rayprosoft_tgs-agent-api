using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;


namespace TGS.Infrastructures.Redis
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddScoped<IOsLog, OsLog>();
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

            return services;
        }
    }
}