// ReSharper disable CheckNamespace

using TGS.Operator.Repositories;
using TGS.Services.Operator.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorConfigurationExtensions
    {
        public static OperatorConfiguration UseOperatorRepositories(this OperatorConfiguration configuration) =>
            configuration
                .UseWalletRepository<WalletRepository>()
                .UseStationLetterRepository<StationLetterRepository>()
                .UseGameProviderRepository<GameProviderRepository>()
                .UseMaintenanceRedisRepository<MaintenanceRedisRepository>();
    }
}