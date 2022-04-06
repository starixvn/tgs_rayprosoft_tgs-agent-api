using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using CoreEnums = TGS.Core.Models.Enums;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class GetPlayerFromClientQueryService : IGetService<GetPlayerFromClientQuery, PlayerInfo>
    {
        private readonly IPlayerServiceClient _playerServiceClient;
        private readonly IVipLevelRepository _vipLevelRepository;

        public GetPlayerFromClientQueryService(
            IPlayerServiceClient playerServiceClient,
            IVipLevelRepository vipLevelRepository)
        {
            _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
            _vipLevelRepository = vipLevelRepository ?? throw new ArgumentNullException(nameof(vipLevelRepository));
        }

        public async ValueTask<PlayerInfo> GetAsync(
            GetPlayerFromClientQuery query,
            CancellationToken cancellationToken = default)
        {
            var player = await _playerServiceClient
                .GetPlayerInfoAsync(query.PlayerCode)
                .ConfigureAwait(false);

            if (player == null)
            {
                throw new PlayerNotFoundException("Player not found.");
            }

            var status = player.Status switch
            {
                PlayerStatus.Enabled => CoreEnums.PlayerStatus.Enabled,
                PlayerStatus.Disabled => CoreEnums.PlayerStatus.Disabled,
                PlayerStatus.InBlacklist => CoreEnums.PlayerStatus.InBlacklist,
                PlayerStatus.RegisterUndone => CoreEnums.PlayerStatus.RegisterUndone,
                _ => throw new ArgumentException("Invalid player status")
            };

            var depositedAmount = await _playerServiceClient
                .GetPlayerDepositedAmountAsync(query.PlayerCode)
                .ConfigureAwait(false);

            var properties =
                await _playerServiceClient
                    .GetPlayerPropertiesAsync(query.PlayerCode)
                    .ConfigureAwait(false);

            var vipLevel = player.VipLevelId is not null
                ? await _vipLevelRepository.GetVipLevelAsync(player.VipLevelId.Value)
                    .ConfigureAwait(false)
                : null;

            return new PlayerInfo(
                player.PlayerCode,
                player.BrandId,
                depositedAmount,
                player.IdentityAuthentication,
                player.PhoneAuthentication,
                properties.GetValueOrDefault("Phone"),
                properties.GetValueOrDefault("AreaCode"),
                player.VipLevelId,
                status,
                player.Username,
                properties.GetValueOrDefault("RealName"),
                properties.GetValueOrDefault("Email"),
                vipLevel ?? 0,
                properties.GetValueOrDefault("QQ"),
                properties.GetValueOrDefault("Wechat"),
                player.IsSetPayPassword);
        }
    }
}
