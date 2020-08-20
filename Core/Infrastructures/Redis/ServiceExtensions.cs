using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;


namespace TGS.Infrastructures.Redis
{
    public static class ServiceExtensions
    {
        private delegate IRedisConnection RedisConnectionMapper(string key);
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            //services.AddSignalR()
            //    .AddMessagePackProtocol(options =>
            //    {
            //        options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
            //        {
            //            MessagePack.Resolvers.StandardResolver.Instance
            //        };
            //    })
            //    .AddStackExchangeRedis(o =>
            //    {
            //        o.ConnectionFactory = async writer =>
            //        {
            //            var config = new ConfigurationOptions
            //            {
            //                AbortOnConnectFail = false
            //            };
            //            config.EndPoints.Add(IPAddress.Loopback, 0);
            //            config.SetDefaultPorts();
            //            var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
            //            connection.ConnectionFailed += (_, e) =>
            //            {
            //                Console.WriteLine("Connection to Redis failed.");
            //            };

            //            if (!connection.IsConnected)
            //            {
            //                Console.WriteLine("Did not connect to Redis.");
            //            }

            //            return connection;
            //        };
            //    });

            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<IOsLog, OsLog>();
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();

            services.AddScoped<IRedisConfig, RedisConfig>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IRedisCacheManager, RedisCacheManager>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));
            services.AddScoped<IRedisPublisher, RedisPublisher>();
            services.AddScoped<IRedisSubscriber, RedisSubscriber>();

            services.AddScoped<IRedisConnection, RedisCoreConnection>();
            services.AddScoped<IRedisConnection, RedisSignalRConnection>();
            services.AddScoped<RedisCoreConnection>();
            services.AddScoped<RedisSignalRConnection>();

            services.AddTransient<Func<string, IRedisConnection>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Core":
                        return serviceProvider.GetRequiredService<RedisCoreConnection>();
                    case "SignalR":
                        return serviceProvider.GetRequiredService<RedisSignalRConnection>();
                    default:
                        return null;
                }
            });

            services.AddTransient<Func<string, IRedisRepository>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Core":
                        return new RedisRepository(serviceProvider.GetRequiredService<RedisCoreConnection>());
                    case "SignalR":
                        return new RedisRepository(serviceProvider.GetRequiredService<RedisSignalRConnection>());
                    default:
                        return null;
                }
            });


            services.AddTransient<Func<string, IRedisCacheManager>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Core":
                        return new RedisCacheManager(new RedisRepository(
                                serviceProvider.GetRequiredService<RedisCoreConnection>()),
                                              serviceProvider.GetService<IJsonSerializationProvider>(),
                            serviceProvider.GetService<IOsLog>());
                    case "SignalR":
                        return new RedisCacheManager(new RedisRepository(
                                serviceProvider.GetRequiredService<RedisSignalRConnection>()),
                            serviceProvider.GetService<IJsonSerializationProvider>(),
                            serviceProvider.GetService<IOsLog>());
                    default:
                        return null;
                }
            });

            services.AddTransient<Func<string, IRedisPublisher>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Core":
                        return new RedisPublisher(
                            serviceProvider.GetRequiredService<RedisCoreConnection>(),
                            serviceProvider.GetService<IJsonSerializationProvider>());
                    case "SignalR":
                        return new RedisPublisher(
                            serviceProvider.GetRequiredService<RedisSignalRConnection>(),
                            serviceProvider.GetService<IJsonSerializationProvider>());
                    default:
                        return null;
                }
            });

            services.AddTransient<Func<string, IRedisSubscriber>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Core":
                        return new RedisSubscriber(
                            serviceProvider.GetRequiredService<RedisCoreConnection>(),
                            serviceProvider.GetService<IJsonSerializationProvider>());
                    case "SignalR":
                        return new RedisSubscriber(
                            serviceProvider.GetRequiredService<RedisSignalRConnection>(),
                            serviceProvider.GetService<IJsonSerializationProvider>());
                    default:
                        return null;
                }
            });

            services.AddTransient<IRedisLock, RedisLock>();

            services.AddTransient<Func<string, IRedisLock>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Core":
                        return new RedisLock(new RedisRepository(serviceProvider.GetRequiredService<RedisCoreConnection>()));
                    case "SignalR":
                        return new RedisLock(new RedisRepository(serviceProvider.GetRequiredService<RedisSignalRConnection>()));
                    default:
                        return null;
                }
            });

            return services;
        }
    }
}