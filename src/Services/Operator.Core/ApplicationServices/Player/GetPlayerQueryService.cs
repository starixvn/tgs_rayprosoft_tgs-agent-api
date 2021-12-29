using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Services.Operator.ApplicationServices.Player
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
                player.PlayerCode,
                player.Username,
                player.RealName,
                player.Phone,
                player.Email,
                player.Vip.Level,
                player.QQ,
                player.Wechat,
                IsSetPayPassword: player.WithdrawPassword.Length > 0,
                IdentityAuthentication: player.IdentityAuthentication);
        }
    }
}
