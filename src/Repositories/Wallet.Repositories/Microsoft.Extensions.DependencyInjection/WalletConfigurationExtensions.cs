using TGS.Services.Wallet.Configuration;
using TGS.Wallet.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WalletConfigurationExtensions
    {
        public static WalletConfiguration AddTgsWalletRepositories(this WalletConfiguration configuration)
            => configuration.AddWalletRepository<WalletRepository>();

        public static WalletConfiguration AddTgsAgentWalletRepositories(this WalletConfiguration configuration)
            => configuration.AddAgentWalletRepository<AgentWalletRepository>();
    }
}