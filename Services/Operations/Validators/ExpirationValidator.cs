using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations.Validators
{
    public interface IExpirationValidator
    {
        Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform = null);
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
            if (loginRecord == null || loginRecord.ExpiredOn < DateTimeOffset.UtcNow)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }
    }
}