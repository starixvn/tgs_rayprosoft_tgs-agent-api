using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

internal class ResetSubAgentPasswordCommandService : ICommandService<ResetSubAgentPasswordCommand>
{
    private readonly IAgentClient _agentClient;

    public ResetSubAgentPasswordCommandService(IAgentClient agentClient)
    {
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
    }

    public async ValueTask ExecuteAsync(ResetSubAgentPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var subAgent = await _agentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(command.SubAgentId), cancellationToken)
            .ConfigureAwait(false);

        if (subAgent is null)
            return;

        if (command.AgentId != subAgent.ParentAgentId)
            return;

        await _agentClient
            .SetAgentPasswordAsync(new SetAgentPasswordRequest(command.SubAgentId, command.Password), cancellationToken)
            .ConfigureAwait(false);
    }
}
