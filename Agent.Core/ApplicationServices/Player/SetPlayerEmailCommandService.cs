using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.PlayerService.Client;
using System.Collections.Generic;
using System.Collections.Immutable;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class SetPlayerEmailCommandService(IAgentClient agentClient, IPlayerServiceClient playerServiceClient) : ICommandService<SetPlayerEmailCommand>
{
    public async ValueTask ExecuteAsync(
        SetPlayerEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var agent = await agentClient.GetAgentInfoAsync(new GetAgentInfoRequest(command.AgentId), cancellationToken);

        await playerServiceClient
            .SetPlayerPropertiesAsync(
                command.PlayerCode,
        new Dictionary<PlayerPropertyName, string?>
                {
                    [PlayerPropertyName.Email] = command.Email
                }.ToImmutableDictionary(),
        agent?.Username,
                cancellationToken)
            .ConfigureAwait(false);
    }
}
