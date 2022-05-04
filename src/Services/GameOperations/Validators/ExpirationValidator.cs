using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.GameOperations.Validators;

public sealed class ExpirationValidator : IExpirationValidator
{
    private readonly IRedisPlayerLoginRecordService _playerLoginRecordService;

    public ExpirationValidator(
        IRedisPlayerLoginRecordService playerLoginRecordService)
    {
        _playerLoginRecordService = playerLoginRecordService ?? throw new ArgumentNullException(nameof(playerLoginRecordService));
    }

    public async Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform)
    {
        var loginRecord = platform == null
            ? await _playerLoginRecordService.GetLastAsync(playerCode).ConfigureAwait(false)
            : await _playerLoginRecordService.GetLastAsync(playerCode, platform.Value).ConfigureAwait(false);

        if (loginRecord == null)
        {
            throw new PlayerLoginExpiredException(playerCode);
        }

        if (loginRecord.ExpiredOn < DateTimeOffset.UtcNow)
        {
            throw new PlayerLoginExpiredException(playerCode);
        }
    }
}
