using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
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

        private readonly IBigGamingApiClient _client;
        private readonly IBigGamingServices _bigGamingServices;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;
        public BigGamingGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IBigGamingApiClient client,
            IBigGamingServices bigGamingServices,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IGameActionQueries gameActionQueries
            ) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _bigGamingServices = bigGamingServices ?? throw new ArgumentNullException(nameof(bigGamingServices));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(round.PlayerCode);
            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(round.GameProviderId, round.BrandId, language);
            var gameActionExternalIds = await _gameActionQueries.GetGameActionExternalIdsAsync(round.GameProviderId, round.PlayerCode, round.ExternalRoundId, round.GameId, new[] { GameActionType.PlaceBet }).ConfigureAwait(false);

            var request = new BigGamingVideoOrderDetailRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                OrderId = Convert.ToInt64(gameActionExternalIds.First().ExternalBetId),
                Locale = cultureCode
            };
            var response = await _client.GetVideoOrderDetailAsync(brandMerchant, request).ConfigureAwait(false);

            var url = default(string);
            if (response.Error == null)
            {
                url = JsonSerializer.Deserialize<string>(JsonSerializer.Serialize(response.Result));
            }

            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new  UrlData() {Url = url}
                }
            };
        }
    }
}