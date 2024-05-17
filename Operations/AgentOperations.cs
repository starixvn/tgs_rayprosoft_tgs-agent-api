using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations;

public sealed class AgentOperations(IAgentQueries agentQueries, IAgentCommands agentCommands) : IAgentOperations
{
    public Task<AgentLoginRecord> AgentLoginAsync(AgentLoginData data) =>
        agentCommands.AgentLoginAsync(data);

    public async Task ChangeAgentPasswordAsync(ChangeAgentPasswordData data)
        => await agentCommands.ChangeAgentPasswordAsync(data);

    public Task<Agent> GetAgentAsync(Guid agentId)
        => agentQueries.GetAgentAsync(x => x.Id == agentId);

    public Task<Agent> GetAgentByInvitationCodeAsync(Guid brandId, string invitationCode)
        => agentQueries.GetAgentByInvitationCodeAsync(brandId, invitationCode);

    public Task<AgentLoginRecord> GetAgentLoginRecordAsync(Guid loginRecordId) =>
        agentQueries.GetAgentLoginRecordAsync(x => x.Id == loginRecordId);

    public Task SetEmailAsync(Guid agentId, string email, DateTimeOffset updatedOn)
        => agentCommands.SetEmailAsync(agentId, email, updatedOn);
}