using System;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Redis;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameProvidersServiceConfigurationExtensions
    {
        public static IServiceCollection AddGameProvidersMognoRepository(
            this IServiceCollection services,
            Action<GameProvidersOption, IServiceProvider> configure)
        {
            services
                .AddOptions<GameProvidersOption>()
                .Configure(configure);

            return services
                .AddTransient<IGameProviderScheduleRepository, GameProviderScheduleRepository>();
        }

        public static IServiceCollection AddGameProviderTCGFtpClientRepository(
            this IServiceCollection services,
            Action<TCGFtpApiOption, IServiceProvider> configure)
        {
            services
                .AddOptions<TCGFtpApiOption>()
                .Configure(configure);
            services
                  .AddHttpClient<GameProviderTCGFtpClientRepository>((sp, c) =>
                  {
                      var options = sp.GetRequiredService<IOptions<TCGFtpApiOption>>().Value;

                      c.BaseAddress = options.Url;
                  });
            return services
                .AddTransient<IGameProviderTCGFtpClientRepository>(sp => sp.GetRequiredService<GameProviderTCGFtpClientRepository>());
        }

        public static IServiceCollection AddGameProvidersoRepository(this IServiceCollection services)
            => services
                .AddTransient<IGameProviderFundTransferTransactionRepository, GameProviderFundTransferTransactionRepository>();

        public static IServiceCollection AddGameProviderScheduleLockRepository(
            this IServiceCollection services
            )
            => services
                 .AddTransient<IGameProviderScheduleLockRepository, GameProviderScheduleLockRepository>();
    }
}
