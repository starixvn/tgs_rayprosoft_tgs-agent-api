using System;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Payment.Clients;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.StationLetters;
using TGS.Services.WalletService.Configuration;

namespace TGS.Services.Operator.Configuration
{
    public class OperatorConfiguration
    {
        public IServiceCollection Services { get; }

        public OperatorConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public OperatorConfiguration AddGameProviderRepository<TGameProviderRepository>()
            where TGameProviderRepository : class, IGameProviderRepository
        {
            Services.AddTransient<IGameProviderRepository, TGameProviderRepository>();

            return this;
        }

        public OperatorConfiguration AddMaintenanceRedisRepository<TMaintenanceRedisRepository>() where TMaintenanceRedisRepository : class, IMaintenanceRedisRepository
        {
            Services
                .AddTransient<IMaintenanceRedisRepository, TMaintenanceRedisRepository>();

            return this;
        }

        public OperatorConfiguration AddPaymentServiceClient(
                            Action<PaymentServiceClientOptions, IServiceProvider> configure)
        {
            Services.AddPaymentServiceClient(configure);

            return this;
        }

        public OperatorConfiguration AddStationLetterRepository<TStationLetterRepository>()
                            where TStationLetterRepository : class, IStationLetterRepository
        {
            Services.AddTransient<IStationLetterRepository, TStationLetterRepository>();

            return this;
        }

        public OperatorConfiguration AddSysRepository<TSysRepository>()
                    where TSysRepository : class, ISysRepository
        {
            Services.AddTransient<ISysRepository, TSysRepository>();

            return this;
        }

        public OperatorConfiguration AddWalletRepository<TWalletRepository>()
            where TWalletRepository : class, IPlayerRepository
        {
            Services
                .AddTransient<IPlayerRepository, TWalletRepository>();

            return this;
        }

        public OperatorConfiguration ConfigureEmailConfiguration<TDep>(Action<EmailConfiguration, TDep> configure)
                            where TDep : class
        {
            Services
                .AddOptions<EmailConfiguration>()
                .Configure(configure);

            return this;
        }

        public OperatorConfiguration ConfigureOperatorOptions<TDep>(Action<OperatorOptions, TDep> configure)
            where TDep : class
        {
            Services
                .AddOptions<OperatorOptions>()
                .Configure(configure);

            return this;
        }

        public OperatorConfiguration ConfigureWallet(Action<WalletConfiguration> configure)
        {
            configure(Services.AddWallet());

            return this;
        }
    }
}
