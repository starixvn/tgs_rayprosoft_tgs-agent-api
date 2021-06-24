using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class GetLogoutQueryService : IGetService<GetLogoutQuery, LogoutInfo>
    {
        private readonly IStatusValidator _statusValidator;
        private readonly IPlayerOperations _playerOperations;

        public GetLogoutQueryService(
            IStatusValidator statusValidator,
            IPlayerOperations playerOperations)
        {
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask<LogoutInfo> GetAsync(GetLogoutQuery query, CancellationToken cancellationToken = default)
        {
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(query.BrandId, ValidStatus.Default);

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(query.PlayerTokenData.PlayerCode);
            if (cachedPlayer == null)
            {
                throw new PlayerNotFoundException("");
            }

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations.PlayerLogoutAsync(
                new PlayerLogoutData()
                {
                    Username = cachedPlayer.Username,
                    PlayerCode = query.PlayerTokenData.PlayerCode,
                    LoginRecordId = query.PlayerTokenData.LoginRecordId,
                    UpdatedOn = updatedOn
                });

            return new LogoutInfo()
            {
                UpdateTime = updatedOn.ToUnixTimeMilliseconds()
            };
        }
    }
}