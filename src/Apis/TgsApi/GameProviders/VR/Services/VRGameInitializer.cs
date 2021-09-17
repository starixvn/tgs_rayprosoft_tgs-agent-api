using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.ApiClients.VR;
using TGS.Shared.Game.Models.VR.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.VR.Services
{
    public record VRGameInitializer : IGameInitializer
    {
        private readonly string _gameProviderCode = Consts.GameProvider.VR;
        private readonly IVRApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly IVRGamePlayerCreate _gamePlayerCreate;
        private readonly ISettingOperations _settingOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public VRGameInitializer(
            IVRApiClient client,
            IPlayerOperations playerOperations,
            IVRGamePlayerCreate gamePlayerCreate,
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gamePlayerCreate = gamePlayerCreate ?? throw new ArgumentNullException(nameof(gamePlayerCreate));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                    .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            if (brandMerchant == null)
            {
                throw new GameProviderApiErrorException("Get configuration brandMerchant is null");
            }

            var dataGetBalance = new VRGetBalanceRequest()
            {
                PlayerCode = player.PlayerCode
            };
            var response = await _client.GetBalanceAsync(brandMerchant, dataGetBalance).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response.playerName) || response.balance == -1)
            {
                await _gamePlayerCreate.CreatePlayerAsync(player).ConfigureAwait(false);
            }

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var channelId = 0;
            if (request.GameId > 0)
            {
                var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(_gameProviderCode).ConfigureAwait(false);
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId).ConfigureAwait(false);
                if (cachedGame != null)
                {
                    channelId = Convert.ToInt32(cachedGame.ExternalId);
                }
            }

            var data = new VRGameUrlRequest()
            {

                PlayerCode = request.Token.PlayerCode,
                LoginTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                IsCreateUser = "true",
                ChannelId = channelId,
                ChannelType = 0
            };

            return new() { Url = _client.GetGameURLAsync(brandMerchant, data).ToString() };
        }
    }
}
