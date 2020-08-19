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
            services.AddScoped<IRedisConfig, RedisConfig>(ReuseWithinContainer);
            services.AddScoped<IRedisConnection, RedisCoreConnection>(ReuseWithinContainer);
            services.AddScoped<IRedisRepository, RedisRepository>(ReuseWithinContainer);
            services.AddScoped<IRedisSourcesFactory, RedisSourcesFactory>(ReuseWithinContainer);
            services.AddScoped<IRedisCacheManager, RedisCacheManager>(ReuseWithinContainer);
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));
            services.AddScoped<IRedisPublisher, RedisPublisher>(ReuseWithinContainer);
            services.AddScoped<IRedisSubscriber, RedisSubscriber>(ReuseWithinContainer);

            services.AddScoped<IRedisConnection, RedisCoreConnection>(RedisConnectionType.Core.ToString());
            services.AddScoped<IRedisConnection, RedisSignalRConnection>(RedisConnectionType.SignalR.ToString());
            container.RegisterFactory<Func<string, IRedisConnection>>(c => new Func<string, IRedisConnection>(code => c.Resolve<IRedisConnection>(code))
                , ReuseWithinContainer);

            container.RegisterFactory<Func<string, IRedisRepository>>(c => new Func<string, IRedisRepository>(
                 code => new RedisRepository(c.Resolve<IRedisConnection>(code)))
                , ReuseWithinContainer);
            container.RegisterFactory<Func<string, IRedisCacheManager>>(c => new Func<string, IRedisCacheManager>(
                code =>
                {
                    var factory = c.Resolve<Func<string, IRedisRepository>>();
                    return new RedisCacheManager(factory(code),
                        c.Resolve<IJsonSerializationProvider>(),
                        c.Resolve<IOsLog>());
                }), ReuseWithinContainer);

            container.RegisterFactory<Func<string, IRedisPublisher>>(c => new Func<string, IRedisPublisher>(
                code => new RedisPublisher(c.Resolve<IRedisConnection>(code), c.Resolve<IJsonSerializationProvider>()))
                , ReuseWithinContainer);

            container.RegisterFactory<Func<string, IRedisSubscriber>>(c => new Func<string, IRedisSubscriber>(
                code => new RedisSubscriber(c.Resolve<IRedisConnection>(code), c.Resolve<IJsonSerializationProvider>()))
                , ReuseWithinContainer);

            services.AddScoped<IRedisLock, RedisLock>(NewEachTime);

            container.RegisterFactory<Func<string, IRedisLock>>(c => new Func<string, IRedisLock>(
                code =>
                {
                    var factory = c.Resolve<Func<string, IRedisRepository>>();
                    return new RedisLock(factory(code));
                }), ReuseWithinContainer);
        }

        public static void SetupRedisWithDependencies(this IUnityContainer container)
        {
            container.SetupJson();
            container.SetupOsLog();
            container.SetupRedis();
        }

        private static ContainerControlledLifetimeManager ReuseWithinContainer => new ContainerControlledLifetimeManager();

        private static ITypeLifetimeManager NewEachTime => new TransientLifetimeManager();
    }
}