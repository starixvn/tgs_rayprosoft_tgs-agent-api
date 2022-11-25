using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.PlayerService.Client;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class SetPlayerEmailCommandService : ICommandService<SetPlayerEmailCommand>
{
    private readonly IPlayerServiceClient _playerServiceClient;

    public SetPlayerEmailCommandService(IPlayerServiceClient playerServiceClient)
    {
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    }

    public async ValueTask ExecuteAsync(
        SetPlayerEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        await _playerServiceClient
            .SetPlayerPropertiesAsync(
                command.PlayerCode,
        new Dictionary<PlayerPropertyName, string?>
                {
                    [PlayerPropertyName.Email] = command.Email
                }.ToImmutableDictionary(),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
