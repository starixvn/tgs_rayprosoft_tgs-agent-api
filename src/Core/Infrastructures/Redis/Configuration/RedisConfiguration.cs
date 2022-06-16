using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Redis.Configuration;

public class RedisConfiguration
{
    public IServiceCollection Services { get; }

    public RedisConfiguration(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public RedisConfiguration RegisterRedisConnectionFactory<TRedisConnectionFactory>()
                where TRedisConnectionFactory : class, IRedisConnectionFactory
    {
        Services.AddSingleton<IRedisConnectionFactory, TRedisConnectionFactory>();

        return this;
    }
}
