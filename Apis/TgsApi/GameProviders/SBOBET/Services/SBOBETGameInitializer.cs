// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Clients;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages.Pluggables;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using static TGS.Apis.TgsApi.GameModels.SBOBET.Enums.ServiceApiError;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETGameInitializer : IGameInitializer
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly ISBOApiClient _client;

        private readonly IReadOnlyDictionary<GameType, string> _portofolios = new Dictionary<GameType, string>
        {
            {GameType.Sportsbook, "SportsBook"},
            {GameType.VirtualSports, "VirtualSports"}
        };

        public SBOBETGameInitializer(
            ISettingOperations settingOperations,
            IBrandOperations brandOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            ISBOApiClient client)
        {
            _settingOperations = settingOperations;
            _brandOperations = brandOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _client = client;
        }

        async Task<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var brandId = request.Player.BrandId;
            var gameProviderId = request.GameProvider.Id;
            var player = request.Player;
            var gameType = request.Game.Type;
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            var response =
                await PlayerLoginAsync(configuration.ApiUrl, configuration.CompanyKey, player, gameType);

            if (response.Error.Code != None)
            {
                return new()
                {
                    Error = (int) TgsApiError.ProviderApiError,
                    ErrorMessage = response.Error.Message
                };
            }

            var platform = request.Token.PlatformType;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            var loginResponse = (LoginResponse) response;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId,
                request.LanguageCode);

            var url = gameType switch
            {
                GameType.VirtualSports => $"https://{loginResponse.Url}&lang={cultureCode}",
                GameType.Sportsbook    => GetSportsbookUrl(bgpc.QueryStrings, platform, loginResponse.Url, cultureCode),
                _                      => default
            };

            return new() {Url = url};
        }

        private async Task<BaseResponse> PlayerLoginAsync(Uri apiUrl, string companyKey, CachedPlayer player,
            GameType gameType)
        {
            if (!_portofolios.TryGetValue(gameType, out var portfolio))
            {
                throw new InvalidOperationException($"No portfolio mapping is found: {gameType}");
            }

            var loginRequest = new LoginRequest
            {
                CompanyKey = companyKey,
                PlayerCode = player.PlayerCode,
                Portfolio = portfolio
            };

            var loginResponse = await _client.LoginAsync(apiUrl, loginRequest);
            if (loginResponse.Error.Code == None || loginResponse.Error.Code != UserNotExist)
            {
                return loginResponse;
            }

            var registerResponse = await _client.RegisterPlayerAsync(apiUrl,
                new(player.BrandCode, player.DefaultWalletCurrencyCode)
                {
                    CompanyKey = companyKey,
                    PlayerCode = player.PlayerCode
                });

            if (registerResponse.Error.Code != None)
            {
                return registerResponse;
            }

            return await _client.LoginAsync(apiUrl, loginRequest);
        }

        private static string GetSportsbookUrl(QueryStrings queryStrings, PlatformType platform,
            string url, string cultureCode)
        {
            var queryString = queryStrings.Desktop;
            var device = "d";

            if (platform == PlatformType.Mobile)
            {
                queryString = queryStrings.Mobile;
                device = "m";
            }

            if (queryString.IsNullOrEmpty())
            {
                queryString = "&oddstyle=MY&theme=sbo&oddsmode=double";
            }

            return $"{url}&device={device}&lang={cultureCode}&{queryString}";
        }
    }
}