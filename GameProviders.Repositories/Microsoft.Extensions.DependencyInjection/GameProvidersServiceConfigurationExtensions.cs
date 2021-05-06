using System;
using GameProviders.Repositories;
using GameProviders.Repositories.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersServiceConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersRepository(this IServiceCollection services, Action<GameProvidersOption, IServiceProvider> configure)
        {
            services
                .AddOptions<GameProvidersOption>()
                .Configure(configure);

            return services
                .AddTransient<IGameProviderScheduleRepository, GameProviderScheduleRepository>();
        }
    }
}
