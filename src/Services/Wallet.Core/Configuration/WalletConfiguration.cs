using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.Wallet.Configuration
{
    public class WalletConfiguration
    {
        public IServiceCollection Services { get; }

        public WalletConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public WalletConfiguration AddWalletRepository<TWalletRepository>() where TWalletRepository : class, IWalletRepository
        {
            Services.AddTransient<IWalletRepository, TWalletRepository>();

            return this;
        }
    }
}
