// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.IMESports;
using TGS.Shared.Game.Models.IMESports.Outgoing;
using static TGS.Shared.Game.Models.IMESports.Enums.GameApiError;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Services
{
    public sealed class IMESportsGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IIMESportsApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public IMESportsGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IIMESportsApiClient client)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _client = client;
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var response = await NewLaunchGameAsync(request);

            if (response.response.ErrorCode != Success)
            {
                throw new GameProviderApiErrorException(response.response.Message);
            }
            
            return new() {Url = response.url};
        }

        private async Task<(BaseResponse response, string url)> NewLaunchGameAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            
            var config =
                await _settingOperations.GetValueFromJsonByKeyAsync<IMESportsConfiguration>(SettingConsts
                    .GameProviderConfigurations.IMESports);
            
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, player.BrandId,
                request.Language);
            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId);

            var apiUrl = config.ApiUrl;
            var platformType = request.Token.PlatformType;
            
            var launchRequest = new NewLaunchGameRequest
            {
                MerchantCode = config.MerchantCode,
                PlayerCode = player.PlayerCode,
                GameId = game.ExternalId,
                Language = cultureCode,
                ProductWallet = config.ProductWallet,
                Http = config.SupportHttps ? 1 : 0
            };

            var launchResponse = await _client.NewLaunchGameAsync(apiUrl, platformType, launchRequest);
            if (launchResponse.ErrorCode == Success || launchResponse.ErrorCode != PlayerDoesNotExist)
            {
                return (launchResponse, launchResponse.GameUrl);
            }

            var registerResponse = await _client.RegisterPlayerAsync(config.ApiUrl,
                new RegisterPlayerRequest
                {
                    MerchantCode = config.MerchantCode,
                    PlayerCode = player.PlayerCode,
                    Currency = player.DefaultWalletCurrencyCode
                });
            if (registerResponse.ErrorCode != Success && registerResponse.ErrorCode != PlayerAlreadyExist)
            {
                return (registerResponse, string.Empty);
            }
            
            launchResponse = await _client.NewLaunchGameAsync(apiUrl, platformType, launchRequest);
            return (launchResponse, launchResponse.GameUrl);
        }
    }
}