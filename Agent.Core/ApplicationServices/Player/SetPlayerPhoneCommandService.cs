using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using Tgs.PlayerService.Client;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TGS.Services.Agent.ApplicationServices.Player;

internal class SetPlayerPhoneCommandService : ICommandService<SetPlayerPhoneCommand>
{
    private readonly IPlayerServiceClient _playerServiceClient;

    public SetPlayerPhoneCommandService(IPlayerServiceClient playerServiceClient)
    {
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    }

    public async ValueTask ExecuteAsync(
        SetPlayerPhoneCommand command,
        CancellationToken cancellationToken = default)
    {
        await _playerServiceClient.SetPlayerPropertiesAsync(
                command.PlayerCode,
                new Dictionary<PlayerPropertyName, string?>
                {
                    [PlayerPropertyName.AreaCode] = command.AreaCode,
                    [PlayerPropertyName.Phone] = command.Phone,
                    [PlayerPropertyName.PhoneAuthentication] = true.ToString()
                }.ToImmutableDictionary(),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
