using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.WalletService.Configuration
{
    public class WalletConfiguration
    {
        public WalletConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public WalletConfiguration AddWalletRepository<TWalletRepository>()
            where TWalletRepository : class, IWalletRepository
        {
            Services.AddTransient<IWalletRepository, TWalletRepository>();

            return this;
        }

        public WalletConfiguration AddAgentWalletRepository<TWalletRepository>()
            where TWalletRepository : class, IAgentWalletRepository
        {
            Services.AddTransient<IAgentWalletRepository, TWalletRepository>();

            return this;
        }
    }
}