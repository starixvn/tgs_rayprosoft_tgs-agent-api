using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class SetPlayerPhoneCommandService(IAgentClient agentClient, IPlayerServiceClient playerServiceClient)
    : ICommandService<SetPlayerPhoneCommand>
{
    public async ValueTask ExecuteAsync(
        SetPlayerPhoneCommand command,
        CancellationToken cancellationToken = default)
    {
        var agent = await agentClient.GetAgentInfoAsync(new GetAgentInfoRequest(command.AgentId), cancellationToken);

        await playerServiceClient.SetPlayerPropertiesAsync(
                command.PlayerCode,
                new Dictionary<PlayerPropertyName, string?>
                {
                    [PlayerPropertyName.AreaCode] = command.AreaCode,
                    [PlayerPropertyName.Phone] = command.Phone,
                    [PlayerPropertyName.PhoneAuthentication] = true.ToString()
                }.ToImmutableDictionary(),
                agent?.Username,
                cancellationToken)
            .ConfigureAwait(false);
    }
}