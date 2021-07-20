using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Mail;
using Tgs.Payment.Clients;
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.StationLetters;
using TGS.Services.WalletService.Configuration;

namespace TGS.Services.Operator.Configuration
{
    public class OperatorConfiguration
    {
        public OperatorConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public OperatorConfiguration AddPaymentServiceClient(
            Action<PaymentServiceClientOptions, IServiceProvider> configure)
        {
            Services.AddPaymentServiceClient(configure);

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

        public OperatorConfiguration UseStationLetterRepository<TStationLetterRepository>()
            where TStationLetterRepository : class, IStationLetterRepository
        {
            Services.AddTransient<IStationLetterRepository, TStationLetterRepository>();

            return this;
        }

        public OperatorConfiguration UseWalletRepository<TWalletRepository>()
            where TWalletRepository : class, IPlayerRepository
        {
            Services
                .AddTransient<IPlayerRepository, TWalletRepository>();

            return this;
        }

        public OperatorConfiguration UseGameProviderRepository<TGameProviderRepository>()
            where TGameProviderRepository : class, IGameProviderRepository
        {
            Services.AddTransient<IGameProviderRepository, TGameProviderRepository>();

            return this;
        }
    }
}