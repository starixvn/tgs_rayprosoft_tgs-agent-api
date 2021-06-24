using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class GetPlayerQueryService : IGetService<GetPlayerQuery, PlayerDataInfo>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;

        public GetPlayerQueryService(
            IPlayerOperations playerOperations,
            IStatusValidator statusValidator)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
        }

        public async ValueTask<PlayerDataInfo> GetAsync(GetPlayerQuery query, CancellationToken cancellationToken = default)
        {
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(query.BrandId, ValidStatus.Default);

            var player = await _playerOperations.GetPlayerByCodeAsync(query.PlayerCode);
            if (player == null)
            {
                throw new PlayerNotFoundException("Player not found.");
            }

            return new PlayerDataInfo(
                player.Username,
                player.RealName,
                player.Email,
                player.Phone,
                player.QQ,
                player.Wechat,
                IsSetPayPassword: player.WithdrawPassword.Length > 0,
                IdentityAuthentication: player.IdentityAuthentication);
        }
    }
}