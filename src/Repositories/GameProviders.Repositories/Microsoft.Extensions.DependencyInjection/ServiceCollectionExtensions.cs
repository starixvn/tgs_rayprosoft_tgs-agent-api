using System;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameProviderScheduleLockRepository(
        this IServiceCollection services
        )
        => services
             .AddTransient<IGameProviderScheduleLockRepository, GameProviderScheduleLockRepository>();

    public static IServiceCollection AddGameProvidersMognoRepository(
                this IServiceCollection services,
        Action<GameProvidersOption, IServiceProvider> configure)
    {
        services
            .AddOptions<GameProvidersOption>()
            .Configure(configure);

        return services;
    }

    public static IServiceCollection AddGameProvidersoRepository(this IServiceCollection services)
        => services
        .AddTransient<IGameProviderFundTransferTransactionRepository, GameProviderFundTransferTransactionRepository>()
        .AddTransient<IPlayerLoginRecordRepository, PlayerLoginRecordRepository>();
}
