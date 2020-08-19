using System;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Redis
{
    public static class UnityContainerExtensions
    {
        public static void SetupRedis(this IUnityContainer container)
        {
            container.RegisterType<IRedisConfig, RedisConfig>(ReuseWithinContainer);
            container.RegisterType<IRedisConnection, RedisCoreConnection>(ReuseWithinContainer);
            container.RegisterType<IRedisRepository, RedisRepository>(ReuseWithinContainer);
            container.RegisterType<IRedisSourcesFactory, RedisSourcesFactory>(ReuseWithinContainer);
            container.RegisterType<IRedisCacheManager, RedisCacheManager>(ReuseWithinContainer);
            container.RegisterType(typeof(IRedisList<>), typeof(RedisList<>), NewEachTime);
            container.RegisterType(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>), NewEachTime);
            container.RegisterType<IRedisPublisher, RedisPublisher>(ReuseWithinContainer);
            container.RegisterType<IRedisSubscriber, RedisSubscriber>(ReuseWithinContainer);

            container.RegisterType<IRedisConnection, RedisCoreConnection>(RedisConnectionType.Core.ToString(), ReuseWithinContainer);
            container.RegisterType<IRedisConnection, RedisSignalRConnection>(RedisConnectionType.SignalR.ToString(), ReuseWithinContainer);
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

            container.RegisterType<IRedisLock, RedisLock>(NewEachTime);

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