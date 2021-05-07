using System;
using GameProviders.Repositories;
using GameProviders.Repositories.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersServiceConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersMognoRepository(this IServiceCollection services, Action<GameProvidersOption, IServiceProvider> configure)
        {
            services
                .AddOptions<GameProvidersOption>()
                .Configure(configure);

            return services
                .AddTransient<IGameProviderScheduleRepository, GameProviderScheduleRepository>();
        }

        public static IServiceCollection AddGameProvidersoRepository(this IServiceCollection services)
        {
            return services
                .AddTransient<IGameProviderFundTransferTransactionRepository, GameProviderFundTransferTransactionRepository>();
        }
    }
}
