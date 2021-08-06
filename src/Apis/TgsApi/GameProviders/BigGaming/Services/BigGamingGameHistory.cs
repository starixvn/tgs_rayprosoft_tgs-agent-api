using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public sealed class BigGamingGameHistory : BaseBetHistory
    {
        private readonly IBigGamingService _service;
        private readonly IBigGamingApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IBigGamingGameHistoryService _bigGamingGameHistoryService;

        public BigGamingGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            IBigGamingApiClient client,
            IBigGamingService service,
            IPlayerOperations playerOperations,
            IBigGamingGameHistoryService bigGamingGameHistoryService,
            IJsonSerializationProvider jsonSerializationProvider
        ) : base(roundQueries, gameProviderOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _bigGamingGameHistoryService = bigGamingGameHistoryService ?? throw new ArgumentNullException(nameof(bigGamingGameHistoryService));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));

        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(round.PlayerCode);
            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode)
                .ConfigureAwait(false);

            if (cachedGame.Type == Core.Models.Enums.GameType.LiveDealer)
            {
                var request = new BigGamingVideoRoundQueryRequest
                {
                    Random = GuidExtensions.NewSequentialGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    Round = round.ExternalRoundId
                };

                var response = await _client.GetVideoRoundQueryAsync(brandMerchant, request).ConfigureAwait(false);

                var value = default(string);
                if (response.Error == null)
                {
                    var gamingVideoRoundQueryResponse =
                        _jsonSerializationProvider.DeserializeFromString<BigGamingVideoRoundQueryResponse>(
                            _jsonSerializationProvider.SerializeToString(response.Result));
                    if (gamingVideoRoundQueryResponse != null)
                    {
                        value = _bigGamingGameHistoryService.GetGameHistory(gamingVideoRoundQueryResponse.Items.First());
                    }
                }

                return new RoundResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                    new JsonData {Value = value}
                    }
                };
            }
            else
            {
                var request = new BigGamingGameRecordFishingUrlRequest
                {
                    Random = GuidExtensions.NewSequentialGuid().ToString(),
                    Sn = brandMerchant.SecretCode,
                    ExternalRoundId = long.Parse(round.ExternalRoundId),
                    GameId = int.Parse(cachedGame.ExternalId)
                };

                var response = await _client.GetGameRecordFishingUrlAsync(brandMerchant, request).ConfigureAwait(false);

                return new RoundResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                    new UrlData {Url = _jsonSerializationProvider.DeserializeFromString<string>(_jsonSerializationProvider.SerializeToString(response.Result))}
                    }
                };
            }

        }

    }
}