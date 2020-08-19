using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Log;


namespace TGS.Infrastructures.Redis
{
    public static class ServiceExtensions
    {
        private delegate IRedisConnection RedisConnectionMapper(string key);
        public static IServiceCollection AddRedisServices(this IServiceCollection services)
        {
            services.AddSignalR()
                .AddMessagePackProtocol(options =>
                {
                    options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
                    {
                        MessagePack.Resolvers.StandardResolver.Instance
                    };
                })
                .AddStackExchangeRedis(o =>
                {
                    o.ConnectionFactory = async writer =>
                    {
                        var config = new ConfigurationOptions
                        {
                            AbortOnConnectFail = false
                        };
                        config.EndPoints.Add(IPAddress.Loopback, 0);
                        config.SetDefaultPorts();
                        var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                        connection.ConnectionFailed += (_, e) =>
                        {
                            Console.WriteLine("Connection to Redis failed.");
                        };

                        if (!connection.IsConnected)
                        {
                            Console.WriteLine("Did not connect to Redis.");
                        }

                        return connection;
                    };
                });

            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<IOsLog, OsLog>();
            services.AddScoped<IJsonSerializationProvider, JsonSerializationProvider>();

            services.AddScoped<IRedisConfig, RedisConfig>();
            services.AddScoped<IRedisConnection, RedisCoreConnection>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            services.AddScoped<IRedisCacheManager, RedisCacheManager>();
            services.AddTransient(typeof(IRedisList<>), typeof(RedisList<>));
            services.AddTransient(typeof(IRedisSortedSet<>), typeof(RedisSortedSet<>));
            services.AddScoped<IRedisPublisher, RedisPublisher>();
            services.AddScoped<IRedisSubscriber, RedisSubscriber>();

            services.AddScoped<RedisCoreConnection>();
            services.AddScoped<RedisSignalRConnection>();


            services.AddSingleton<RedisConnectionMapper>(provider => key =>
            {
                switch ((string.IsNullOrEmpty(key) ? " " : key).ToUpper())
                {
                    case "Core":
                        return provider.GetRequiredService<RedisCoreConnection>();
                    case "SignalR":
                        return provider.GetRequiredService<RedisSignalRConnection>();
                    default:
                        return null;
                }
            });


            //services.AddScoped<Func<string, IRedisConnection>>(c => new Func<string, IRedisConnection>(
            //    code => c.GetService<IRedisConnection>(code)));
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