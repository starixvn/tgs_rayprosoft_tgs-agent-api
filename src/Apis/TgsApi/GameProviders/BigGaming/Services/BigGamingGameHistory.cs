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

        public BigGamingGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IBigGamingApiClient client,
            IBigGamingServices bigGamingServices,
            IPlayerOperations playerOperations,
            IBrandGameProviderOperations brandGameProviderOperations
            ) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _bigGamingServices = bigGamingServices ?? throw new ArgumentNullException(nameof(bigGamingServices));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(round.PlayerCode);
            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(round.GameProviderId, round.BrandId, language);

            var request = new BigGamingVideoRoundQueryRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                Round = round.ExternalRoundId
            };
            var response = await _client.GetVideoRoundQueryAsync(brandMerchant, request).ConfigureAwait(false);

            var value = default(string);
            if (response.Error == null)
            {
                var gamingVideoRoundQueryResponse = JsonSerializer.Deserialize<BigGamingVideoRoundQueryResponse>(JsonSerializer.Serialize(response.Result));
                if (gamingVideoRoundQueryResponse != null)
                {
                    value = GetGameHistory(gamingVideoRoundQueryResponse.Items.First());
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

        private string GetGameHistory(BigGamingVideoRoundQueryItemResponse source) => source.GameType switch
        {

            2 => source.Result,
            3 => source.Result,
            4 => source.Result,
            5 => source.Baccarat64,
            6 => source.Result,
            8 => source.Result,
            11 => string.Join(',', Convert.FromBase64String(source.Baccarat64)),
            13 => string.Join(',', Convert.FromBase64String(source.Baccarat64)),
            14 => string.Join(',', Convert.FromBase64String(source.Baccarat64)),
            15 => source.Result,
            _ => GetBaccaratGameHistory(Convert.FromBase64String(source.Baccarat64))
        };
        private string GetBaccaratGameHistory(byte[] result)
        {
            string[] banker = new string[3];
            string[] player = new string[3];
            int bankerIndex = 0;
            int playerIndex = 0;
            for (int i = 0; i < result.Length; i++)
            {
                int value = result[i];
                string card = string.Empty;
                string number = string.Empty;
                int cardIndex = (value / 13);

                if (value > 0)
                {
                    card = GetCard(cardIndex);
                    number = GetNumber(value - (13 * (cardIndex == 4 ? cardIndex - 1 : cardIndex)));
                }


                if (i is 0 or 2 or 4)
                {

                    player[playerIndex] = card + number;
                    playerIndex++;
                }
                else
                {

                    banker[bankerIndex] = card + number;
                    bankerIndex++;
                }
            }
            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "player",string.Join(',', player)},
                        { "banker",string.Join(',', banker)}
                               });
        }

        private string GetCard(int cardIndex) => cardIndex switch
        {
            1 => "♥",
            2 => "♣",
            3 => "♦",
            4 => "♦",
            _ => "♠"
        };
        private string GetNumber(int value) => value switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => value.ToString()
        };
    }
}