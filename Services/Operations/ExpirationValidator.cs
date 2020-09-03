using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using UGS.Core.Models.Exceptions;

namespace TGS.Services.Operations
{
    public interface IExpirationValidator
    {
        Task ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform = null);
    }

    public sealed class ExpirationValidator : IExpirationValidator
    {
        private readonly IPlayerOperations _playerOpertions;

        public ExpirationValidator(IPlayerOperations playerOperations)
        {
            _playerOpertions = playerOperations;
        }

        async Task IExpirationValidator.ThrowIfPlayerLoginExpiredAsync(string playerCode, PlatformType? platform)
        {
            var loginRecord = await _playerOpertions.GetPlayerLatestEnabledLoginRecordAsync(playerCode, platform);
            if (loginRecord == null || loginRecord.ExpiredOn < DateTimeOffset.UtcNow)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }
        }
    }
}