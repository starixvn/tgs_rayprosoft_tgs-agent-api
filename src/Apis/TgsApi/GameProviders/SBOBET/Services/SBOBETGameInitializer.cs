// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Clients;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.Models.SBOBET.Outgoing;
using static TGS.Shared.Game.Models.SBOBET.Enums.ServiceApiError;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly ISBOApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public SBOBETGameInitializer(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            ISBOApiClient client)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _client = client;
        }

        async Task<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);
            var gameType = await _gameProviderOperations.GetGameTypeAsync(gameProviderId, request.GameId);

            if (!gameType.HasValue)
            {
                throw new InvalidOperationException($"Game with id '{request.GameId}' doesn't exist");
            }

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            var response =
                await PlayerLoginAsync(configuration.ApiUrl, configuration.CompanyKey, player, gameType.Value);

            if (response.Error.Code != None)
            {
                throw new GameProviderApiErrorException(response.Error.Message);
            }

            var brandId = player.BrandId;
            var platform = request.Token.PlatformType;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            var loginResponse = (LoginResponse) response;
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId,
                request.Language);

            var url = gameType switch
            {
                GameType.VirtualSports or GameType.VirtualSportsLobby => $"{loginResponse.Url}&lang={cultureCode}",
                GameType.Sportsbook => GetSportsbookUrl(bgpc.QueryStrings, platform, loginResponse.Url, cultureCode),
                _ => default
            };

            return new() {Url = url};
        }

        private async Task<BaseResponse> PlayerLoginAsync(Uri apiUrl, string companyKey, CachedPlayer player,
            GameType gameType)
        {
            var portfolio = Utility.GetPortfolio(gameType);

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
                queryString = "oddstyle=MY&theme=sbo&oddsmode=double";
            }

            return $"{url}&device={device}&lang={cultureCode}&{queryString}";
        }
    }
}