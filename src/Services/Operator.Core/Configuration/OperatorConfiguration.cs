using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Mail;

namespace TGS.Services.Operator.Configuration
{
    public class OperatorConfiguration
    {
        public IServiceCollection Services { get; }

        public OperatorConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public OperatorConfiguration ConfigureOperatorOptions<TDep>(Action<OperatorOptions, TDep> configure) where TDep : class
        {
            Services
                .AddOptions<OperatorOptions>()
                .Configure(configure);

            return this;
        }

        public OperatorConfiguration ConfigureEmailConfiguration<TDep>(Action<EmailConfiguration, TDep> configure) where TDep : class
        {
            Services
                .AddOptions<EmailConfiguration>()
                .Configure(configure);

            return this;
        }

        public OperatorConfiguration UseDepositRepository<TDepositRepository>() where TDepositRepository : class, IWalletRepository
        {
            Services
                .AddTransient<IWalletRepository, TDepositRepository>();

            return this;
        }
    }
}
