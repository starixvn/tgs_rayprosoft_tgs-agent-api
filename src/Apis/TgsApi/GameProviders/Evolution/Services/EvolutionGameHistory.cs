using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.Evolution;
using TGS.Shared.Game.Models.Evolution.Commons.EvolutionGamHistoryRound.GameResult;
using TGS.Shared.Game.Models.Evolution.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Services
{
    public sealed class EvolutionGameHistory : BaseBetHistory
    {
        private readonly IEvolutionApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public EvolutionGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IEvolutionApiClient client,
            ISettingOperations settingOperations,
            IJsonSerializationProvider jsonSerializationProvider
            ) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _client = client ?? throw new System.ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new System.ArgumentNullException(nameof(settingOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new System.ArgumentNullException(nameof(jsonSerializationProvider));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {

            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<EvolutionConfiguration>(SettingConsts.GameProviderConfigurations.Evolution).ConfigureAwait(false);
            var response = await _client.GetGamHistoryRoundAsync(configuration, new EvolutionGamHistoryRoundRequest() { PlayerCode = playerCode, RoundId = round.ExternalRoundId }).ConfigureAwait(false);

            if (cachedGame.Type == GameType.Slot)
            {
                var gameResult = _jsonSerializationProvider.DeserializeFromString<SlotGames>(_jsonSerializationProvider.SerializeToString(response.Data.Result));
                var path = gameResult.Path[1..];
                var url = $"{configuration.ApiUrl}{path}";
                return new RoundResult()
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                        new UrlData {Url=url}
                    }
                };
            }
            else
            {
                return new RoundResult
                {
                    BetAmount = round.BetAmount ?? 0,
                    Turnover = round.Turnover ?? 0,
                    Winloss = round.WinLoss ?? 0,
                    GameResult = round.ClientStatus.ToString(),
                    Data = new BaseData[]
                    {
                        new JsonData { Value =GetGameHistory( response)  }
                    }
                };
            }
        }

        private string GetGameHistory(EvolutionGamHistoryRoundResponse source) => source.Data.GameType.ToLower() switch
        {
            "sicbo" => GetSicboGameHistory(source.Data.Result),//骰寶
            "roulette" => GetRouletteGameHistory(source.Data.Result),//輪盤
            "dragontiger" => GetDragontigerGameHistory(source.Data.Result),//轮盘
            "baccarat" => GetBaccaratGameHistory(source.Data.Result),//百家樂相關
            "blackjack" => GetBlackJackGameHistory(source.Data.Result),//21點
            _ => _jsonSerializationProvider.SerializeToString(source.Data.Result)
        };

        private string GetRouletteGameHistory(JsonDocument source)
        {
            var gameResult = _jsonSerializationProvider.DeserializeFromString<Roulette>(_jsonSerializationProvider.SerializeToString(source));
            var roulettes = (from Outcome in gameResult.Outcomes
                             select new
                             {
                                 number = Outcome.Number,
                                 color = Outcome.Color
                             }).ToArray();

            return _jsonSerializationProvider.SerializeToString(roulettes);
        }

        private string GetBlackJackGameHistory(JsonDocument source)
        {

            var gameResult = _jsonSerializationProvider.DeserializeFromString<BlackJack>(_jsonSerializationProvider.SerializeToString(source));

            var result = new Dictionary<string, object>();


            if (gameResult.Dealer != null)
            {
                var cards = (from card in gameResult.Dealer.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("dealer", new { score = gameResult.Dealer.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat1 != null)
            {
                var cards = (from card in gameResult.Seats.Seat1.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat1", new { score = gameResult.Seats.Seat1.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat2 != null)
            {
                var cards = (from card in gameResult.Seats.Seat2.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat2", new { score = gameResult.Seats.Seat2.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat3 != null)
            {
                var cards = (from card in gameResult.Seats.Seat3.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat3", new { score = gameResult.Seats.Seat3.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat4 != null)
            {
                var cards = (from card in gameResult.Seats.Seat4.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat4", new { score = gameResult.Seats.Seat4.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat5 != null)
            {
                var cards = (from card in gameResult.Seats.Seat5.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat5", new { score = gameResult.Seats.Seat5.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat6 != null)
            {
                var cards = (from card in gameResult.Seats.Seat6.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat6", new { score = gameResult.Seats.Seat6.Score, cards = string.Join(',', cards) });
            }

            if (gameResult.Seats.Seat7 != null)
            {
                var cards = (from card in gameResult.Seats.Seat7.Cards
                             select
                                 GetCard(card).ToString()
                             ).ToArray();
                result.Add("seat7", new { score = gameResult.Seats.Seat7.Score, cards = string.Join(',', cards) });
            }

            return JsonSerializer.Serialize(result);
        }


        private string GetBaccaratGameHistory(JsonDocument source)
        {

            var gameResult = _jsonSerializationProvider.DeserializeFromString<Baccarat>(_jsonSerializationProvider.SerializeToString(source));

            var bankers = (from card in gameResult.Banker.Cards
                           select
                               GetCard(card).ToString()
                           ).ToArray();

            var players = (from card in gameResult.Player.Cards
                           select
                               GetCard(card).ToString()
                         ).ToArray();

            return JsonSerializer.Serialize(
                new Dictionary<string, string>() {
                    { "player",string.Join(',', players)},
                    { "banker",string.Join(',', bankers)}
                });
        }

        private string GetDragontigerGameHistory(JsonDocument source)
        {
            var gameResult = _jsonSerializationProvider.DeserializeFromString<DragonTiger>(_jsonSerializationProvider.SerializeToString(source));
            return JsonSerializer.Serialize(
                new Dictionary<string, string>() {
                    { "dragon",GetCard(gameResult.Dragon.Card)},
                    { "tiger", GetCard(gameResult.Tiger.Card)}
                });
        }

        private string GetSicboGameHistory(JsonDocument source)
        {
            var gameResult = _jsonSerializationProvider.DeserializeFromString<SicBo>(_jsonSerializationProvider.SerializeToString(source));

            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, int>() {
                    { "dice1",gameResult.First},
                    { "dice2",gameResult.Second},
                    { "dice3", gameResult.Third}
                });
        }

        private string GetCard(string rank) => $"{GetSuit(rank.ToCharArray()[1])}{GetScore(rank.ToCharArray()[0])}";

        private string GetSuit(char suit) => suit switch
        {
            'H' => "♥",
            'C' => "♣",
            'D' => "♦",
            _ => "♠"
        };

        private string GetScore(char score) => score switch
        {
            'T' => "10",
            _ => score.ToString()
        };
    }
}
