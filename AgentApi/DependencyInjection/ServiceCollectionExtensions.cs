using System;
using TGS.Apis.AgentApi;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> configure)
    {
        services
            .AddOptions<RedisOptions>()
            .Configure(configure)
            .Services
            .AddRedisServices()
            .RegisterRedisConnectionFactory<RedisConnectionFactory>();

        return services;
    }
}