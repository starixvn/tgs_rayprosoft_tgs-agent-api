using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Tokens;
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
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly ISabaApiClient _client;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public SabaGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            ISabaApiClient client)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
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
                await GetGameUrlAsync(configuration.ApiUrl, configuration.VendorId, gameProviderId, player, token);

            if (response.Code != GetGameUrlApi.Ok)
            {
                throw new GameProviderApiErrorException(response.ErrorMessage);
            }

            var culture =
                await _brandGameProviderOperations.MapCultureAsync(gameProviderId, player.BrandId, request.Language);

            var url = response.Url
                .AppendQueryString($"tsId={token.LoginRecordId}")
                .AppendQueryString($"lang={culture}");

            return new() {Url = url};
        }

        private async ValueTask<GameUrlResponse> GetGameUrlAsync(Uri apiUrl, string vendorId, Guid gameProviderId,
            CachedPlayer player, PlayerTokenData token)
        {
            var playerCode = player.PlayerCode;
            var platform = token.PlatformType == PlatformType.Mobile ? "2" : "1";

            var gameUrlRequest = new GameUrlRequest
            {
                VendorId = vendorId,
                Platform = platform,
                PlayerCode = playerCode
            };

            var gameUrlResponse = await _client.GetGameUrlAsync(apiUrl, gameUrlRequest);
            if (gameUrlResponse.Code is GetGameUrlApi.Ok or not GetGameUrlApi.MemberNotExist)
            {
                return gameUrlResponse;
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

            return await _client.GetGameUrlAsync(apiUrl, gameUrlRequest);
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