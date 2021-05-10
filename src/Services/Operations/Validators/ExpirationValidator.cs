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
        Task ThrowIfAgentLoginExpiredAsync(Guid loginRecordId, Guid agentId);
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
            VerifyLoginRecordStatus(loginRecord?.Status, playerCode);
            VerifyLoginRecordExpiredTime(loginRecord?.ExpiredOn, playerCode);
        }

        Task IExpirationValidator.ThrowIfAgentLoginExpiredAsync(Guid loginRecordId, Guid agentId) =>
            throw new NotImplementedException();

        private static void VerifyLoginRecordPlayer(string loginRecordPlayerCode, string playerCode)
        {
            if (loginRecordPlayerCode != playerCode)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }

        private static void VerifyLoginRecordStatus(LoginStatus? status, string playerCode)
        {
            if (!status.HasValue || status.Value != LoginStatus.Enabled)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }

        private static void VerifyLoginRecordExpiredTime(DateTimeOffset? expiredOn, string playerCode)
        {
            if (!expiredOn.HasValue || expiredOn.Value < DateTimeOffset.UtcNow)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }
    }
}