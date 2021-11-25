// ReSharper disable CheckNamespace

using TGS.Operator.Repositories;
using TGS.Services.Operator.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorConfigurationExtensions
    {
        public static OperatorConfiguration UseOperatorRepositories(this OperatorConfiguration configuration)
        {
            configuration.Services
                .AddSingleton<RedisConnectionFactory>();

            configuration.Services
                .AddSingleton<ISystemClock, DefaultSystemClock>();

            return configuration
                .AddWalletRepository<PlayerRepository>()
                .AddStationLetterRepository<StationLetterRepository>()
                .AddGameProviderRepository<GameProviderRepository>()
                .AddMaintenanceRedisRepository<MaintenanceRedisRepository>()
                .AddSysRepository<SysRepository>()
                .AddHelp2PayRedisRepository<Help2PayRedisRepository>();
        }
    }
}
