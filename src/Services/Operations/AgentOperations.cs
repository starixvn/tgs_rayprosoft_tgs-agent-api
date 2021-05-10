using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IAgentOperations
    {
        Task<Agent> GetAgentAsync(string invitationCode);
    }

    public sealed class AgentOperations : IAgentOperations
    {
        private readonly IAgentQueries _agentQueries;
        private readonly IRedisCacheManager _cacheManager;

        public AgentOperations(
            IRedisCacheManager cacheManager,
            IAgentQueries agentQueries)
        {
            _cacheManager = cacheManager;
            _agentQueries = agentQueries;
        }

        public Task<Agent> GetAgentAsync(string invitationCode) =>
            _agentQueries.GetAgentAsync(invitationCode);
    }
}