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

    public ValueTask ExecuteAsync(ResetSubAgentPasswordCommand command, CancellationToken cancellationToken = default)
    => _agentClient.ResetSubAgentPasswordAsync(
        new ResetSubAgentPasswordRequest(command.AgentId, command.SubAgentId, command.Password), cancellationToken);
}
