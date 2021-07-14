using System;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Payment.Clients;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.WalletService.Configuration;

namespace TGS.Services.Agent.Configuration
{
    public class AgentConfiguration
    {
        public IServiceCollection Services { get; }

        public AgentConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public AgentConfiguration AddAgentRepository<TAgentRepository>() where TAgentRepository : class, IAgentRepository
        {
            Services.AddTransient<IAgentRepository, TAgentRepository>();

            return this;
        }

        public AgentConfiguration AddPaymentServiceClient(Action<PaymentServiceClientOptions, IServiceProvider> configure)
        {
            Services.AddPaymentServiceClient(configure);

            return this;
        }

        public AgentConfiguration ConfigureAgentOptions<TDep>(Action<AgentOptions, TDep> configure) where TDep : class
        {
            Services
                .AddOptions<AgentOptions>()
                .Configure(configure);

            return this;
        }

        public AgentConfiguration ConfigureEmailConfiguration<TDep>(Action<EmailConfiguration, TDep> configure) where TDep : class
        {
            Services
                .AddOptions<EmailConfiguration>()
                .Configure(configure);

            return this;
        }

        public AgentConfiguration ConfigureWallet(Action<WalletConfiguration> configure)
        {
            configure(Services.AddWallet());

            return this;
        }
    }
}
