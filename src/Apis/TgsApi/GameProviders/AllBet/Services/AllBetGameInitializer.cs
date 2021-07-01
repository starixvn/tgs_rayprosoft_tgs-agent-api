using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.AllBet;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.AllBet;
using TGS.Shared.Game.Models.AllBet.Enums;
using TGS.Shared.Game.Models.AllBet.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public class AllBetGameInitializer : IGameInitializer
    {
        private static readonly string _ResultOk = ServiceApiError.OK.ToString();
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

            var response = await GetGameUrlAsync(playerId, language, configuration.ApiUrl, agent)
                .ConfigureAwait(false);

            if (response.ErrorCode.IgnoreCaseEquals(_ResultOk))
            {
                var loginResponse = (LoginResponse)response;
                return new GetGameUrlResponse {Url = loginResponse.Data.Url};
            }

            var errorMessage = response.ErrorMessage.IsNullOrEmpty()
                ? response.ErrorCode
                : response.ErrorMessage;

            throw new GameProviderApiErrorException(errorMessage);
        }

        private ValueTask<CheckOrCreateMemberResponse> CheckOrCreateMemberAsync(string playerId, Uri apiUrl,
            AllBetAgent agent)
        {
            var request = new CheckOrCreateMemberRequest
            {
                AgentId = agent.AgentId,
                PlayerId = playerId
            };

            return _apiClient.CheckOrCreateMemberAsync(
                apiUrl,
                agent.PropertyId,
                agent.AllBetApiKey,
                request);
        }

        private ValueTask<LoginResponse> LoginAsync(string playerId, string language, Uri apiUrl, AllBetAgent agent)
        {
            var request = new LoginRequest
            {
                PlayerId = playerId,
                Language = language
            };

            return _apiClient.LoginAsync(
                apiUrl,
                agent.PropertyId,
                agent.AllBetApiKey,
                request);
        }

        private async ValueTask<ErrorResponse> GetGameUrlAsync(string playerId, string language, Uri apiUrl,
            AllBetAgent agent)
        {
            var loginResponse = await LoginAsync(playerId, language, apiUrl, agent)
                .ConfigureAwait(false);

            if (loginResponse.ErrorCode.IgnoreCaseEquals(_ResultOk) ||
                loginResponse.ErrorCode.IgnoreCaseNotEquals(ServiceApiError.PLAYER_NOT_EXIST.ToString()))
            {
                return loginResponse;
            }

            var checkOrCreateMemberResponse = await CheckOrCreateMemberAsync(playerId, apiUrl, agent)
                .ConfigureAwait(false);

            if (checkOrCreateMemberResponse.ErrorCode.IgnoreCaseNotEquals(_ResultOk) ||
                checkOrCreateMemberResponse.ErrorCode.IgnoreCaseNotEquals(ServiceApiError.PLAYER_EXIST.ToString()))
            {
                return checkOrCreateMemberResponse;
            }

            return await LoginAsync(playerId, language, apiUrl, agent)
                .ConfigureAwait(false);
        }
    }
}