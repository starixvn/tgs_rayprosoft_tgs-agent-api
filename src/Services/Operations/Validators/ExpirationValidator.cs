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
        Task ThrowIfAgentLoginExpiredAsync(Guid loginRecordId);
    }

    public sealed class ExpirationValidator : IExpirationValidator
    {
        private readonly IAgentOperations _agentOperations;
        private readonly IPlayerOperations _playerOperations;

        public ExpirationValidator(IPlayerOperations playerOperations, IAgentOperations agentOperations)
        {
            _playerOperations = playerOperations;
            _agentOperations = agentOperations;
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

        async Task IExpirationValidator.ThrowIfAgentLoginExpiredAsync(Guid loginRecordId)
        {
            var agentLoginRecord = await _agentOperations.GetAgentLoginRecordAsync(loginRecordId);

            if (agentLoginRecord is null || agentLoginRecord.ExpiredOn < DateTimeOffset.UtcNow)
            {
                throw new("Agent login token is expired");
            }
        }

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