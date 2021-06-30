using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.AllBet;
using TGS.Shared.Game.Models.AllBet.Enums;
using TGS.Shared.Game.Models.AllBet.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public class AllBetGameInitializer : IGameInitializer
    {
        private readonly IAllBetAgentMapper _agentMapper;
        private readonly IAllBetApiClient _apiClient;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public AllBetGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IAllBetApiClient apiClient,
            IAllBetAgentMapper agentMapper)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _apiClient = apiClient;
            _agentMapper = agentMapper;
        }


        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AllBetConfiguration>(SettingConsts
                    .GameProviderConfigurations.AllBet);

            var agent = _agentMapper.GetAgent(configuration.BrandMerchants, player.BrandCode,
                player.DefaultWalletCurrencyCode);

            if (agent == null)
            {
                throw new ArgumentException(
                    $"No agent of '{player.BrandCode}' and '{player.DefaultWalletCurrencyCode}' is found");
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode)
                .ConfigureAwait(false);

            var language = await _brandGameProviderOperations
                .MapCultureAsync(gameProviderId, player.BrandId, request.Language).ConfigureAwait(false);

            var playerId = $"{player.PlayerCode}{agent.Suffix}";

            var loginRequest = new LoginRequest
            {
                PlayerId = playerId,
                Language = language
            };

            var loginResponse = await _apiClient.LoginAsync(configuration.ApiUrl, agent.PropertyId,
                agent.AllBetApiKey, loginRequest);

            if (loginResponse.ErrorCode.IgnoreCaseEquals(ServiceApiError.OK.ToString()))
            {
                return new GetGameUrlResponse {Url = loginResponse.Data.Url};
            }

            var message = loginResponse.ErrorMessage.IsNullOrEmpty()
                ? loginResponse.ErrorCode
                : loginResponse.ErrorMessage;

            throw new GameProviderApiErrorException(message);
        }
    }
}