using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.Jili;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public class JiliGameHistory : BaseBetHistory
{
    private readonly IBrandGameProviderOperations _brandGameProviderOperations;
    private readonly IJiliApiClient _client;
    private readonly IJiliMerchantMapper _merchantMapper;
    private readonly IPlayerOperations _playerOperations;
    private readonly IJiliService _service;

    public JiliGameHistory(
        IRoundQueries roundQueries,
        IGameProviderOperations gameProviderOperations,
        IBrandGameProviderOperations brandGameProviderOperations,
        IPlayerOperations playerOperations,
        IJiliService service,
        IJiliMerchantMapper merchantMapper,
        IJiliApiClient client)
        : base(roundQueries, gameProviderOperations)
    {
        if (roundQueries == null) throw new ArgumentNullException(nameof(roundQueries));
        if (gameProviderOperations == null) throw new ArgumentNullException(nameof(gameProviderOperations));
        _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
        _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(
        string playerCode,
        Round round,
        CachedGameProvider cachedGameProvider,
        CachedGame cachedGame,
        string language)
    {
        var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);

        var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);
        var merchant = _merchantMapper.GetMerchant(configuration.Merchants, player.BrandCode);

        var culture = await _brandGameProviderOperations
            .MapCultureAsync(
                cachedGameProvider.Id,
                player.BrandId,
                language)
            .ConfigureAwait(false);

        var response = await _client.GetGameDetailUrlAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                culture,
                round.ExternalRoundId)
            .ConfigureAwait(false);

        if (response.ErrorCode != 0)
        {
            throw new GameProviderApiErrorException($"Get jili bet history fail: {response.ErrorMessage}");
        }

        return new()
        {
            BetAmount = round.BetAmount ?? 0,
            Winloss = (round.WinLoss ?? 0).Negate(),
            Turnover = round.Turnover ?? 0,
            GameResult = round.ClientStatus.ToString(),
            Data = new BaseData[]
            {
                new UrlData
                {
                    Url = response.Data.Url
                }
            }
        };
    }
}
