using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class SetPlayerEmailCommandService(IAgentClient agentClient, IPlayerServiceClient playerServiceClient)
    : ICommandService<SetPlayerEmailCommand>
{
    public async ValueTask ExecuteAsync(
        SetPlayerEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var agent = await agentClient.GetAgentInfoAsync(new GetAgentInfoRequest(command.AgentId), cancellationToken);

        await playerServiceClient
            .SetPlayerPropertiesAsync(
                command.PlayerCode,
                new Dictionary<PlayerPropertyName, string?> { [PlayerPropertyName.Email] = command.Email }
                    .ToImmutableDictionary(),
                agent?.Username,
                cancellationToken)
            .ConfigureAwait(false);
    }
}