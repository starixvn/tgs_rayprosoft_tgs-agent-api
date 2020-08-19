using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Redis
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRedisServices(this IServiceCollection services)
        {
            services.AddScoped<IRedisConfig, RedisConfig>();
            services.AddScoped<IRedisConnection, RedisCoreConnection>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IRedisCacheManager, RedisCacheManager>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));
            services.AddScoped<IRedisPublisher, RedisPublisher>();
            services.AddScoped<IRedisSubscriber, RedisSubscriber>();

            //services.AddScoped<IRedisConnection, RedisCoreConnection>(RedisConnectionType.Core.ToString());
            //services.AddScoped<IRedisConnection, RedisSignalRConnection>(RedisConnectionType.SignalR.ToString());
            services.AddScoped<Func<string, IRedisConnection>>(c => new Func<string, IRedisConnection>(code => c.GetService<IRedisConnection>()));

            services.AddScoped<Func<string, IRedisRepository>>(c => new Func<string, IRedisRepository>(
                 code => new RedisRepository(c.GetService<IRedisConnection>())));
            services.AddScoped<Func<string, IRedisCacheManager>>(c => new Func<string, IRedisCacheManager>(
                code =>
                {
                    var factory = c.GetService<Func<string, IRedisRepository>>();
                    return new RedisCacheManager(factory(code),
                        c.GetService<IJsonSerializationProvider>(),
                        c.GetService<IOsLog>());
                }));

            //services.AddScoped<Func<string, IRedisPublisher>>(c => new Func<string, IRedisPublisher>(
            //    code => new RedisPublisher(c.GetService<IRedisConnection>(code), c.GetService<IJsonSerializationProvider>())));

            //services.AddScoped<Func<string, IRedisSubscriber>>(c => new Func<string, IRedisSubscriber>(
            //    code => new RedisSubscriber(c.GetService<IRedisConnection>(code), c.GetService<IJsonSerializationProvider>())));

            services.AddTransient<IRedisLock, RedisLock>();

            services.AddScoped<Func<string, IRedisLock>>(c => new Func<string, IRedisLock>(
                code =>
                {
                    var factory = c.GetService<Func<string, IRedisRepository>>();
                    return new RedisLock(factory(code));
                }));
            return services;
        }
    }
}