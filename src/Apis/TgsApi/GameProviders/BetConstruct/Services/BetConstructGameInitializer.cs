using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.BetConstruct;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Services
{
    public sealed class BetConstructGameInitializer : IGameInitializer
    {
        private static readonly Dictionary<string, string> _externalBackendCultureCodeToFrontendLanguageCode = new Dictionary<string, string>
        {
            { "en" , "eng" },//English
            { "zh" , "chi" },//Chinese
            { "ru" , "rus"},//Russian
            { "lt" , "lit"},//Lithuanian
            { "ka" , "geo"},//Georgian
            { "tr" , "tur"},//Turkish
            { "fr" , "fre"},//French
            { "es" , "spa"},//Spanish
            { "sr" , "srp"},//Serbian
            { "et" , ""},//Estonian
            { "lv" , ""},//Latvian
            { "me" , "" },//Montenegrian
        };

        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IBetConstructApiClient _client;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBetConstructService _service;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public BetConstructGameInitializer(
            IBrandGameProviderOperations brandGameProviderOperations,
            IBetConstructApiClient client,
            IPlayerOperations playerOperations,
            IBetConstructService service,
            ISettingOperations settingOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            ITokenProvider tokenProvider,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations)
        {
            _brandGameProviderOperations = brandGameProviderOperations
                ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var encryptedToken = await _tokenProvider.EncryptAsync(request.Token.PlayerCode);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var brandId = player.BrandId;
            var platform = request.Token.PlatformType;

            var externalBackendCultureCode =
                await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId, request.Language);

            _ = _externalBackendCultureCodeToFrontendLanguageCode.TryGetValue(externalBackendCultureCode, out var frontendLanguageCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<BetConstructConfiguration>(SettingConsts
                    .GameProviderConfigurations.BetConstruct);

            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            var gpc = await _gameProviderOperations.GetCachedGameProviderConfigurationAsync(gameProviderId,
                bgpc.GameProviderConfigurationId);

            //var currencyCode =
            //    await _currencyCodeMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);
            var currencyCode =
                await _currencyCodeMapper.InternalToExternalAsync("VND", gameProviderId);

            var domain = platform == PlatformType.Mobile ? gpc.MobileEndpoint : gpc.DesktopEndpoint;

            var endpointPath = "/js/partnerinit.js?containerID​={CONTAINERID}&UserId​={USERID}&AuthToken​={AUTHTOKEN}&lang={LANG}&sport​={SPORT}";

            var url = domain.AppendQueryString(endpointPath)
                    .Replace("{CONTAINERID}", "bcsportsbookcontainer")
                    .Replace("{USERID}", configuration.PartnerId)
                    .Replace("{AUTHTOKEN}", encryptedToken)
                    .Replace("{LANG}", frontendLanguageCode)
                    .Replace("{SPORT}", "1");

            return new()
            {
                Url = url
            };
        }
    }
}
