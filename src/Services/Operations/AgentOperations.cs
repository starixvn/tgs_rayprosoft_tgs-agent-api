using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IAgentOperations
    {
        Task<Agent> GetAgentAsync(Guid agentId);
        Task<AgentLoginRecord> GetAgentLoginRecordAsync(Guid loginRecordId);
        Task<Agent> GetAgentByInvitationCodeAsync(string invitationCode);
        Task<AgentLoginRecord> AgentLoginAsync(AgentLoginData data);
    }

    public sealed class AgentOperations : IAgentOperations
    {
        private readonly IAgentCommands _agentCommands;
        private readonly IAgentQueries _agentQueries;

        public AgentOperations(IAgentQueries agentQueries, IAgentCommands agentCommands)
        {
            _agentQueries = agentQueries;
            _agentCommands = agentCommands;
        }

        public Task<Agent> GetAgentAsync(Guid agentId) =>
            _agentQueries.GetAgentAsync(x => x.Id == agentId);

        Task<AgentLoginRecord> IAgentOperations.AgentLoginAsync(AgentLoginData data) =>
            _agentCommands.AgentLoginAsync(data);

        Task<Agent> IAgentOperations.GetAgentByInvitationCodeAsync(string invitationCode) =>
            _agentQueries.GetAgentByInvitationCodeAsync(invitationCode);

        Task<AgentLoginRecord> IAgentOperations.GetAgentLoginRecordAsync(Guid loginRecordId) =>
            _agentQueries.GetAgentLoginRecordAsync(x => x.Id == loginRecordId);
    }
}