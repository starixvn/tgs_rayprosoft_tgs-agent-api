using System;
using TGS.Apis.TgsApi;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(
            this IServiceCollection service,
            Action<RedisOptions, IServiceProvider> configure)
        {
            service
                .AddOptions<RedisOptions>()
                .Configure(configure)
                .Services
                .AddRedisServices()
                .RegisterRedisConnectionFactory<RedisConnectionFactory>();

            return service;
        }
    }
}
