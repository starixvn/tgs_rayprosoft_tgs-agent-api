using TGS.Agent.Repositories;
using TGS.Services.Agent.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AgentConfigurationExtensions
    {
        public static AgentConfiguration AddTgsAgentRepository(this AgentConfiguration configuration)
            => configuration.AddAgentRepository<AgentRepository>();
    }
}
