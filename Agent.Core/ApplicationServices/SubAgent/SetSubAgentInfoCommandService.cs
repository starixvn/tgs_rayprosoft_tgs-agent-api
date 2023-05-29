using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

internal class SetSubAgentInfoCommandService : ICommandService<SetSubAgentInfoCommand>
{
    private readonly IAgentClient _agentClient;

    public SetSubAgentInfoCommandService(IAgentClient agentClient)
    {
        _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));
    }

    public ValueTask ExecuteAsync(SetSubAgentInfoCommand command, CancellationToken cancellationToken = default)
        => _agentClient.SetSubAgentInfoAsync(
            new SetSubAgentInfoRequest(
                command.AgentId,
                command.SubAgentId,
                command.RealName,
                command.Email,
                command.Phone), cancellationToken);
}
