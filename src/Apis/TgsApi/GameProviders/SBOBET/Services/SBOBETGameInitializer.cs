// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

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
using TGS.Shared.Game.ApiClients.SBOBET;
using TGS.Shared.Game.Models.SBOBET.Outgoing;
using static TGS.Shared.Game.Models.SBOBET.Enums.ServiceApiError;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services;

public sealed class SBOBETGameInitializer : IGameInitializer
{
    private readonly IBrandGameProviderOperations _brandGameProviderOperations;
    private readonly IBrandOperations _brandOperations;
    private readonly ISBOApiClient _client;
    private readonly IGameProviderCurrencyCodeMapper _gameProviderCurrencyCodeMapper;
    private readonly IGameProviderOperations _gameProviderOperations;
    private readonly IPlayerOperations _playerOperations;
    private readonly ISettingOperations _settingOperations;

    public SBOBETGameInitializer(
        ISettingOperations settingOperations,
        IPlayerOperations playerOperations,
        IBrandOperations brandOperations,
        IGameProviderOperations gameProviderOperations,
        IBrandGameProviderOperations brandGameProviderOperations,
        IGameProviderCurrencyCodeMapper gameProviderCurrencyCodeMapper,
        ISBOApiClient client)
    {
        _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));

        _gameProviderOperations =
            gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));

        _brandGameProviderOperations = brandGameProviderOperations ??
            throw new ArgumentNullException(nameof(brandGameProviderOperations));

        _gameProviderCurrencyCodeMapper = gameProviderCurrencyCodeMapper ??
            throw new ArgumentNullException(nameof(gameProviderCurrencyCodeMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
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
            await PlayerLoginAsync(gameProviderId, player, gameType.Value, configuration);

        if (response.Error.Code != None)
        {
            throw new GameProviderApiErrorException(response.Error.Message);
        }

        var brandId = player.BrandId;
        var platform = request.Token.PlatformType;
        var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

        var loginResponse = (LoginResponse)response;

        var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId,
            brandId,
            request.Language);

        var url = gameType switch
        {
            GameType.VirtualSports or GameType.VirtualSportsLobby => $"{loginResponse.Url}&lang={cultureCode}",
            GameType.Sportsbook => GetSportsbookUrl(bgpc.QueryStrings, platform, loginResponse.Url, cultureCode),
            _ => default
        };

        return new()
        {
            Url = url
        };
    }

    private async ValueTask<BaseResponse> PlayerLoginAsync(
        Guid gameProviderId,
        CachedPlayer player,
        GameType gameType,
        SBOConfiguration configuration)
    {
        var portfolio = Utility.GetPortfolio(gameType);

        var loginRequest = new LoginRequest
        {
            CompanyKey = configuration.CompanyKey,
            PlayerCode = player.PlayerCode,
            Portfolio = portfolio
        };

        var apiUrl = configuration.ApiUrl;
        var loginResponse = await _client.LoginAsync(apiUrl, loginRequest).ConfigureAwait(false);

        if (loginResponse.Error.Code == None || loginResponse.Error.Code != UserNotExist)
        {
            return loginResponse;
        }

        var createPlayerResponse =
            await CreatePlayerAsync(gameProviderId, player, configuration).ConfigureAwait(false);

        if (createPlayerResponse.Error.Code != None)
        {
            return createPlayerResponse;
        }

        return await _client.LoginAsync(apiUrl, loginRequest);
    }

    private async ValueTask<BaseResponse> CreatePlayerAsync(
        Guid gameProviderId, CachedPlayer player, SBOConfiguration configuration)
    {
        var createPlayerRequest = new RegisterPlayerRequest(
            configuration.AgentPrefix,
            player.BrandCode,
            player.DefaultWalletCurrencyCode)
        {
            CompanyKey = configuration.CompanyKey,
            PlayerCode = player.PlayerCode
        };

        var apiUrl = configuration.ApiUrl;

        var registerResponse = await _client.CreatePlayerAsync(apiUrl, createPlayerRequest)
            .ConfigureAwait(false);

        if (registerResponse.Error.Code is None or not AgentNotExist)
        {
            return registerResponse;
        }

        var gameProviderCurrency =
            await _gameProviderCurrencyCodeMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode,
                gameProviderId).ConfigureAwait(false);

        var createAgentResponse = await _client.CreateAgentAsync(apiUrl,
            new CreateAgentRequest
            {
                AgentId = createPlayerRequest.AgentId,
                CurrencyCode = gameProviderCurrency,
                CompanyKey = configuration.CompanyKey
            }).ConfigureAwait(false);

        if (createAgentResponse.Error.Code != None)
        {
            return createAgentResponse;
        }

        return await _client.CreatePlayerAsync(apiUrl, createPlayerRequest)
            .ConfigureAwait(false);
    }

    private static string GetSportsbookUrl(
        QueryStrings queryStrings, PlatformType platform,
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