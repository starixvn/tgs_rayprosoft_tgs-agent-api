using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Infrastructures.Mail;

namespace TGS.Services.Agent.Configuration
{
    public class AgentConfiguration
    {
        public IServiceCollection Services { get; }

        public AgentConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public AgentConfiguration ConfigureEmailConfiguration<TDep>(Action<EmailConfiguration, TDep> configure) where TDep : class
        {
            Services
                .AddOptions<EmailConfiguration>()
                .Configure(configure);

            return this;
        }

        public AgentConfiguration ConfigureAgentOptions<TDep>(Action<AgentOptions, TDep> configure) where TDep : class
        {
            Services
                .AddOptions<AgentOptions>()
                .Configure(configure);

            return this;
        }
    }
}