using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Saba;
using TGS.Shared.Game.Models.Saba.Enums;
using TGS.Shared.Game.Models.Saba.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public sealed class SabaGameInitializer : IGameInitializer
    {
        private readonly IBrandOperations _brandOperations;
        private readonly ISabaApiClient _client;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public SabaGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            ISabaApiClient client)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _currencyCodeMapper = currencyCodeMapper;
            _client = client;
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;

            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SabaConfiguration>(SettingConsts
                    .GameProviderConfigurations.Saba);

            var response =
                await PlayerLoginAsync(configuration.ApiUrl, configuration.VendorId, gameProviderId, player);

            if (response.ErrorCode != LoginApiError.Ok)
            {
                throw new GameProviderApiErrorException(response.ErrorMessage);
            }

            var brandId = player.BrandId;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            var gpc = await _gameProviderOperations.GetCachedGameProviderConfigurationAsync(gameProviderId,
                bgpc.GameProviderConfigurationId);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId);

            var isMobile = token.PlatformType == PlatformType.Mobile;
            var domain = isMobile ? gpc.MobileEndpoint : gpc.DesktopEndpoint;

            var url = domain.AppendQueryString(game.EndpointPath)
                .Replace("{GPTOKEN}", response.Token)
                .Replace("{TOKENID}", token.LoginRecordId.ToString());

            return new() {Url = url};
        }

        private async ValueTask<LoginResponse> PlayerLoginAsync(Uri apiUrl, string vendorId, Guid gameProviderId,
            CachedPlayer player)
        {
            var playerCode = player.PlayerCode;

            var loginRequest = new LoginRequest
            {
                VendorId = vendorId,
                PlayerCode = playerCode
            };

            var loginResponse = await _client.LoginAsync(apiUrl, loginRequest);
            if (loginResponse.ErrorCode is LoginApiError.Ok or not LoginApiError.MemberNotExist)
            {
                return loginResponse;
            }

            var oddsType = GetOddsType(player.DefaultWalletCurrencyCode);
            var currencyCode =
                await _currencyCodeMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            var createMemberResponse = await _client.CreateMemberAsync(apiUrl,
                new()
                {
                    VendorId = vendorId,
                    PlayerCode = playerCode,
                    Username = playerCode,
                    CurrencyCode = currencyCode,
                    OddsType = oddsType
                });

            if (createMemberResponse.ErrorCode != CreateMemberApiError.Ok)
            {
                throw new GameProviderApiErrorException(createMemberResponse.ErrorMessage);
            }

            return await _client.LoginAsync(apiUrl, loginRequest);
        }

        private static string GetOddsType(string playerCurrencyCode)
        {
            var oddType = playerCurrencyCode switch
            {
                "USD"      => OddsType.American,
                "VND_1000" => OddsType.Malay,
                "IDR_1000" => OddsType.Indo,
                "KRW"      => OddsType.Decimal,
                "MYR"      => OddsType.Malay,
                "THB"      => OddsType.Malay,
                "GBP"      => OddsType.Decimal,
                "JPY"      => OddsType.Decimal,
                "CNY"      => OddsType.China,
                _          => OddsType.Malay
            };

            return ((int)oddType).ToString();
        }
    }
}