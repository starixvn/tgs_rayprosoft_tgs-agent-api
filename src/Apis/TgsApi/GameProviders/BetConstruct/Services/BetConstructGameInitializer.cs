using System;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.BetConstruct;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;

public sealed class BetConstructGameInitializer : IGameInitializer
{
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

    ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
    {
        var url = "/api/game/bc";

        return new ValueTask<GetGameUrlResponse>(
            new GetGameUrlResponse()
            {
                Url = url
            });
    }
}
