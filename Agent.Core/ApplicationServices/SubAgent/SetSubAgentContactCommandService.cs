using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Client.Models.Enums;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.SubAgent;

internal class SetSubAgentContactCommandService(IAgentClient agentClient) : ICommandService<SetSubAgentContactCommand>
{
    private readonly IAgentClient _agentClient = agentClient ?? throw new ArgumentNullException(nameof(agentClient));

    public async ValueTask ExecuteAsync(SetSubAgentContactCommand command,
        CancellationToken cancellationToken = default)
    {
        var subAgent = await _agentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(command.SubAgentId), cancellationToken)
            .ConfigureAwait(false);

        if (subAgent is null || command.AgentId != subAgent.ParentAgentId)
            return;

        await _agentClient
            .SetAgentContactAsync(new SetAgentContactRequest(
                    command.SubAgentId,
                    command.Email,
                    command.Phone),
                cancellationToken)
            .ConfigureAwait(false);

        var parentAgent = await _agentClient
            .GetAgentInfoAsync(new GetAgentInfoRequest(command.AgentId), cancellationToken)
            .ConfigureAwait(false);

        await _agentClient
            .AddAgentAuditLogAsync(
                new AddAgentAuditLogRequest(
                    command.SubAgentId,
                    AgentOperation.Contact,
                    parentAgent!.Username),
                cancellationToken)
            .ConfigureAwait(false);
    }
}