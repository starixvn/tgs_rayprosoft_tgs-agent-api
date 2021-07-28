using System.Reflection;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Agent;
using TGS.Services.Agent.Configuration;
using TGS.Services.Agent.ValidateCode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AgentServiceCollectionExtensions
    {
        public static AgentConfiguration AddAgentApiCore(this IServiceCollection services)
        {
            services.RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);

            services
                .AddOptions<AgentOptions>()
                .Services
                .AddValidateCodeClient((options, sp) =>
                {
                    var agentOptions = sp.GetRequiredService<IOptions<AgentOptions>>().Value;

                    options.ValidateCodeBaseUrl = agentOptions.ValidateCodeBaseUrl;
                })
                .AddTransient<IValidateCodeOperator, RestValidateCodeOperator>()
                .AddSingleton<IValidateCodeGenerator, RandomValidateCodeGenerator>()
                .AddMail();

            var opConfiguration = new AgentConfiguration(services);

            return opConfiguration;
        }
    }
}
