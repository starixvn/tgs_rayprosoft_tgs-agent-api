using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.AsiaGaming;
using TGS.Shared.Game.Models.AsiaGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public sealed class AsiaGamingGameInitializer : IGameInitializer
    {
        private const string Separator = "/\\\\/";
        private readonly IAsiaGamingApiClient _apiClient;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly ISymmetricCryptoProvider _cryptoProvider;
        private readonly IGameProviderCurrencyCodeMapper _currencyMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public AsiaGamingGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyMapper,
            IAsiaGamingApiClient apiClient,
            ISymmetricCryptoProviderFactory cryptoProviderFactory,
            IHashProvider hashProvider,
            IJsonSerializationProvider json)
        {
            _settingOperations = settingOperations;
            _brandOperations = brandOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _currencyMapper = currencyMapper;
            _apiClient = apiClient;
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.DES);
            _hashProvider = hashProvider;
            _json = json;
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);
            var cur = await _currencyMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(SettingConsts
                    .GameProviderConfigurations.AsiaGaming);

            var isValidAccount = await CheckOrCreateAccountAsync(configuration, player, cur);
            if (isValidAccount.Result != "0")
            {
                throw new InvalidOperationException(
                    $"CheckOrCreateAccount error {isValidAccount.Result} to player {request.Token.PlayerCode}");
            }

            var balance = await _playerOperations.GetPlayerBalanceAsync(player.DefaultWalletId, false);
            var result = await CreateGameSessionAsync(configuration, request, balance);
            if (result.Error != "OK")
            {
                throw new InvalidOperationException(
                    $"CreateGameSession error {result.Error} to player {request.Token.PlayerCode}");
            }

            var url = await GetGameUrlAsync(configuration, player, request, gameProviderId, cur);
            return new() {Url = url};
        }

        private async Task<CheckOrCreateAccountResponse> CheckOrCreateAccountAsync(
            AsiaGamingConfiguration configuration, CachedPlayer player, string cur)
        {
            var request = new CheckOrCreateAccountRequest
            {
                AgentCode = configuration.AgentCode,
                PlayerCode = player.PlayerCode,
                Currency = cur
            };

            var paras = GetQueryString(request, Separator);
            var encodedParas = await _cryptoProvider.EncryptStringAsync(paras,
                configuration.DesKey, configuration.DesKey,
                CipherMode.ECB, PaddingMode.PKCS7, Encoding.UTF8);
            var key = _hashProvider.Hash(encodedParas + configuration.Md5Key,
                HashAlgorithmKind.Md5, HashStringKind.Hex, Encoding.UTF8);

            return await _apiClient.CheckOrCreateAccountAsync(configuration.LoginApiUrl, encodedParas, key);
        }

        private async Task<PlayerTicketResponse> CreateGameSessionAsync(
            AsiaGamingConfiguration configuration, GetGameUrlRequest urlRequest, decimal balance)
        {
            var request = new CreateGameSessionRequest
            {
                ProductId = configuration.ProductionId,
                PlayerCode = urlRequest.Token.PlayerCode,
                LoginRecordId = urlRequest.Token.LoginRecordId,
                Balance = balance
            };

            var queryString = GetQueryString(request, "&");

            return await _apiClient.CreateGameSessionAsync(configuration.ApiUrl, queryString);
        }

        private async Task<string> GetGameUrlAsync(AsiaGamingConfiguration configuration, CachedPlayer player,
            GetGameUrlRequest urlRequest, Guid gameProviderId, string cur)
        {
            var brandId = player.BrandId;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId,
                urlRequest.Language);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, urlRequest.GameId);

            var request = new GameUrlRequest
            {
                AgentCode = configuration.AgentCode,
                PlayerCode = urlRequest.Token.PlayerCode,
                Sid = configuration.AgentCode + GenerateSequenceNumber(),
                Lang = cultureCode,
                GameId = game.ExternalId,
                Currency = cur
            };

            var paras = GetQueryString(request, Separator);
            var encodedParas = await _cryptoProvider.EncryptStringAsync(paras,
                configuration.DesKey, configuration.DesKey,
                CipherMode.ECB, PaddingMode.PKCS7, Encoding.UTF8);
            var key = _hashProvider.Hash(encodedParas + configuration.Md5Key,
                HashAlgorithmKind.Md5, HashStringKind.Hex, Encoding.UTF8);

            var platform = urlRequest.Token.PlatformType;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            var gpc = await _gameProviderOperations.GetCachedGameProviderConfigurationAsync(gameProviderId,
                bgpc.GameProviderConfigurationId);
            var domain = platform == PlatformType.Mobile ? gpc.MobileEndpoint : gpc.DesktopEndpoint;

            return domain.AppendQueryString(game.EndpointPath)
                .Replace("{PARAS}", encodedParas)
                .Replace("{KEY}", key);
        }

        private string GetQueryString<T>(T data, string separator)
        {
            var index = 0;
            var parameters = string.Empty;
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var value = _json.SerializeToString(data, options);
            var items = _json.DeserializeFromString<Dictionary<string, object>>(value, options);

            foreach (var item in items)
            {
                parameters += item.Key + "=" + item.Value;
                index++;

                if (index < items.Count)
                {
                    parameters += separator;
                }
            }

            return parameters;
        }

        private string GenerateSequenceNumber() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    }
}