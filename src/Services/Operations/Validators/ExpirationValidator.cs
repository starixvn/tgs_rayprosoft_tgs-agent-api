using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations.Validators
{
    public sealed class ExpirationValidator : IExpirationValidator
    {
        private readonly IAgentOperations _agentOperations;
        private readonly IPlayerQueries _playerQueries;

        public ExpirationValidator(
            IPlayerQueries playerQueries,
            IAgentOperations agentOperations)
        {
            _playerQueries = playerQueries ?? throw new ArgumentNullException(nameof(playerQueries));
            _agentOperations = agentOperations;
        }

        public async Task ThrowIfAgentLoginExpiredAsync(Guid loginRecordId)
        {
            var agentLoginRecord = await _agentOperations.GetAgentLoginRecordAsync(loginRecordId);

            if (agentLoginRecord is null || agentLoginRecord.ExpiredOn < DateTimeOffset.UtcNow)
            {
                throw new("Agent login token is expired");
            }
        }

        public async Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform)
        {
            var loginRecords = await _playerQueries
                .GetPlayerLoginsRecordsInOneDayAsync(playerCode, LoginStatus.Enabled)
                .ConfigureAwait(false);

            var loginRecord = loginRecords?
                .OrderByDescending(r => r.ExpiredOn)
                .FirstOrDefault(r => platform == null || r.PlatformType == platform);

            VerifyLoginRecordExpiredTime(loginRecord?.ExpiredOn, playerCode);
        }

        public async Task ThrowIfPlayerLoginExpiredAsync(Guid loginRecordId, string playerCode)
        {
            var loginRecord = await _playerQueries.GetPlayerLoginRecordAsync(loginRecordId).ConfigureAwait(false);
            VerifyLoginRecordPlayer(loginRecord?.PlayerCode, playerCode);
            VerifyLoginRecordStatus(loginRecord?.Status, playerCode);
            VerifyLoginRecordExpiredTime(loginRecord?.ExpiredOn, playerCode);
        }

        private static void VerifyLoginRecordExpiredTime(DateTimeOffset? expiredOn, string playerCode)
        {
            if (!expiredOn.HasValue || expiredOn.Value < DateTimeOffset.UtcNow)
            {
                throw new PlayerLoginExpiredException(playerCode);
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
    }
}
