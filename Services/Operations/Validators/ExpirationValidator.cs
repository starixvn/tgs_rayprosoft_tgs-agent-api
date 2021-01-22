using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations.Validators
{
    public interface IExpirationValidator
    {
        Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform = null);

        Task ThrowIfPlayerLoginExpiredAsync(Guid loginRecordId, string playerCode);
    }

    public sealed class ExpirationValidator : IExpirationValidator
    {
        private readonly IPlayerOperations _playerOperations;

        public ExpirationValidator(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations;
        }

        async Task IExpirationValidator.ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform)
        {
            var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform);
            VerifyLoginRecordExpiredTime(loginRecord?.ExpiredOn, playerCode);
        }

        async Task IExpirationValidator.ThrowIfPlayerLoginExpiredAsync(Guid loginRecordId, string playerCode)
        {
            var loginRecord = await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
            VerifyLoginRecordPlayer(loginRecord?.PlayerCode, playerCode);
            VerifyLoginRecordExpiredTime(loginRecord?.ExpiredOn, playerCode);
        }

        private static void VerifyLoginRecordPlayer(string loginRecordPlayerCode, string playerCode)
        {
            if (loginRecordPlayerCode != playerCode)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }

        private static void VerifyLoginRecordExpiredTime(DateTimeOffset? expiredOn, string playerCode)
        {
            if (!expiredOn.HasValue || expiredOn < DateTimeOffset.UtcNow)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }
    }
}