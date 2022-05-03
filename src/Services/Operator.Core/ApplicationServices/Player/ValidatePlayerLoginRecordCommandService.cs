using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class ValidatePlayerLoginRecordCommandService : ICommandService<ValidatePlayerLoginRecordCommand>
{
    private readonly IPlayerRepository _playerRepository;

    public ValidatePlayerLoginRecordCommandService(
        IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    public async ValueTask ExecuteAsync(ValidatePlayerLoginRecordCommand command, CancellationToken cancellationToken = default)
    {
        var loginRecord = await _playerRepository.GetPlayerLoginRecordAsync(command.LoginRecordId)
            .ConfigureAwait(false);

        if (loginRecord == null)
        {
            throw new PlayerLoginExpiredException(command.PlayerCode);
        }

        if (loginRecord.PlayerCode != command.PlayerCode)
        {
            throw new PlayerLoginExpiredException(command.PlayerCode);
        }

        if (loginRecord.Status != LoginStatus.Enabled)
        {
            throw new PlayerLoginExpiredException(command.PlayerCode);
        }

        if (loginRecord.ExpiredOn < DateTimeOffset.UtcNow)
        {
            throw new PlayerLoginExpiredException(command.PlayerCode);
        }
    }
}
