using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Card365;
using TGS.Shared.Game.Models.Card365.Enums;
using TGS.Shared.Game.Models.Card365.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public class Card365GameInitializer : IGameInitializer
    {
        private readonly ICard365AgentMapper _agentMapper;
        private readonly ICard365ApiClient _apiClient;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public Card365GameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            ITokenProvider tokenProvider,
            ICard365AgentMapper agentMapper,
            ICard365ApiClient apiClient)
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _brandGameProviderOperations = brandGameProviderOperations ??
                throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _agentMapper = agentMapper ?? throw new ArgumentNullException(nameof(agentMapper));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async ValueTask<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode).ConfigureAwait(false);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode)
                .ConfigureAwait(false);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<Card365Configuration>(SettingConsts
                    .GameProviderConfigurations.Card365);

            var agent = _agentMapper.GetAgent(configuration.BrandAgents, player.BrandCode);

            if (agent == null)
            {
                throw new ArgumentException(
                    $"No agent of '{player.BrandCode}' is found");
            }

            var accessKey = await GetKeyAsync(configuration.ApiUrl, configuration.Cert, agent.AgentId,
                configuration.Website, gameProviderId, player).ConfigureAwait(false);

            var language = await _brandGameProviderOperations
                .MapCultureAsync(gameProviderId, player.BrandId, request.Language).ConfigureAwait(false);

            var url = await GetGameUrlAsync(configuration.ApiUrl, configuration.Website, agent.AgentId, accessKey,
                player, language, request.Token).ConfigureAwait(false);

            return new GetGameUrlResponse { Url = url };
        }

        private async ValueTask<string> GetKeyAsync(Uri apiUrl, string cert, string agentId, string website,
            Guid gameProviderId, CachedPlayer player)
        {
            var currencyCode = await _currencyCodeMapper
                .InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId).ConfigureAwait(false);

            var request = new GetKeyRequest
            {
                Cert = cert,
                AgentId = agentId,
                PlayerCode = player.PlayerCode,
                Username = player.Username,
                CurrencyCode = currencyCode
            };

            var response = await _apiClient.GetAccessKeyAsync(apiUrl, website, request)
                .ConfigureAwait(false);

            if (response.Status == (int)ServiceApiError.Success)
            {
                return response.Key;
            }

            var status = (ServiceApiError)response.Status;
            throw new GameProviderApiErrorException($"status code: {response.Status}, message: {status}");
        }

        private async ValueTask<string> GetGameUrlAsync(Uri apiUrl, string website, string agentId, string accessKey,
            CachedPlayer player, string language, PlayerTokenData token)
        {
            var encryptedToken = await _tokenProvider.EncryptAsync(token.LoginRecordId);

            var request = new LoginRequest
            {
                PlayerCode = player.PlayerCode,
                AccessKey = accessKey,
                AgentId = agentId,
                Username = player.Username,
                Token = encryptedToken,
                Language = language,
                EnableFreeGame = false,
                EnableJackpot = false,
                EnableReward = false,
                EnableFullscreen = false
            };

            var json = JsonSerializer.Serialize(request);
            var collection = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            var builder = new StringBuilder();
            foreach (var (key, value) in collection!)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.Append(key);
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(value));
            }

            var queryString = builder.ToString();
            return $"{apiUrl.AbsoluteUri.TrimEnd('/')}/{website}/loginV2?{queryString}";
        }
    }
}