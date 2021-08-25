using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.WMCasino;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.WMCasino;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public class WMCasinoGameInitializer : IGameInitializer
    {
        private readonly IWMCasinoAgentMapper _agentMapper;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IWMCasinoApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IWMCasinoPlayerServices _playerServices;
        private readonly ISettingOperations _settingOperations;

        public WMCasinoGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IWMCasinoAgentMapper agentMapper,
            IWMCasinoPlayerServices playerServices,
            IWMCasinoApiClient client)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _agentMapper = agentMapper;
            _playerServices = playerServices;
            _client = client;
        }

        public async ValueTask<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<WMCasinoConfiguration>(SettingConsts
                    .GameProviderConfigurations.WMCasino);

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

            var response = await GetGameUrlAsync(player, language, configuration.ApiUrl, agent)
                .ConfigureAwait(false);

            if (response.ErrorCode == (int)ServiceApiError.Success && response is LoginGameResponse loginResponse)
            {
                return new GetGameUrlResponse { Url = loginResponse.Url };
            }

            throw new GameProviderApiErrorException(response.ErrorMessage);
        }

        private async ValueTask<ErrorResponse> GetGameUrlAsync(CachedPlayer player, string language, Uri apiUrl,
            WMCasinoAgent agent)
        {
            var playerId = _playerServices.ToWMCasinoPlayerId(player.PlayerCode, agent.Prefix, agent.Suffix);

            var loginGameResponse = await LoginGameAsync(playerId, language, apiUrl, agent)
                .ConfigureAwait(false);

            if (loginGameResponse.ErrorCode is (int)ServiceApiError.Success
                or not (int)ServiceApiError.UserNotExist)
            {
                return loginGameResponse;
            }

            var memberRegisterResponse = await MemberRegisterAsync(playerId, player.Username, apiUrl, agent)
                .ConfigureAwait(false);

            if (memberRegisterResponse.ErrorCode is not (int)ServiceApiError.Success
                and not (int)ServiceApiError.UserExist)
            {
                return memberRegisterResponse;
            }

            return await LoginGameAsync(playerId, language, apiUrl, agent)
                .ConfigureAwait(false);
        }

        private ValueTask<LoginGameResponse> LoginGameAsync(string playerId, string language, Uri apiUrl,
            WMCasinoAgent agent)
        {
            var request = new LoginGameRequest
            {
                VendorId = agent.AgentId,
                Signature = agent.Signature,
                User = playerId,
                Language = language
            };

            return _client.LoginGameAsync(apiUrl, request);
        }

        private ValueTask<MemberRegisterResponse> MemberRegisterAsync(string playerId, string username, Uri apiUrl,
            WMCasinoAgent agent)
        {
            var request = new MemberRegisterRequest
            {
                VendorId = agent.AgentId,
                Signature = agent.Signature,
                User = playerId,
                Username = username.Length > 30 ? username[..30] : username
            };

            return _client.MemberRegisterAsync(apiUrl, request);
        }
    }
}