using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public sealed class SexyGamingGameHistory : BaseBetHistory
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IRoundQueries _roundQueries;
        private Dictionary<string, string> _pokerMapper;

        public SexyGamingGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations
        ) : base(roundQueries, gameProviderOperations)
        {
            _roundQueries = roundQueries ?? throw new ArgumentNullException(nameof(roundQueries));
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            InitoPoker();
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var gameRecord = JsonSerializer.Deserialize<SexyGamingGameRecord>(round.RoundDetail.AdditionalData);
            var value = default(string);
            if (gameRecord != null)
            {
                var games = await _gameProviderOperations.GetCachedGamesAsync(round.GameProviderId)
                    .ConfigureAwait(false);
                var externalGameId = games.Where(x => x.Id == round.GameId).First().ExternalId;
                value = GetGameHistory(externalGameId, gameRecord);
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


        private string GetGameHistory(string externalGameId, SexyGamingGameRecord source)
            => externalGameId switch
            {
                "MX-LIVE-001" => GetBaccaratGameHistory(source.Result),    //Baccarat Classic
                "MX-LIVE-002" => GetBaccaratGameHistory(source.Result),    //Baccarat
                "MX-LIVE-003" => GetBaccaratGameHistory(source.Result),    //Baccarat Insurance
                "MX-LIVE-006" => GetDragontigerGameHistory(source.Result), //DragonTiger
                "MX-LIVE-007" => GetSicboGameHistory(source.Result),       //SicBo
                "MX-LIVE-009" => JsonSerializer.Serialize(source.Result),  //Roulette
                "MX-LIVE-010" => GetRedBlueDuelGameHistory(source.Result), //Red Blue Duel
                "MX-LIVE-011" => JsonSerializer.Serialize(source.Result),  //Twenty
                "MX-LIVE-012" => JsonSerializer.Serialize(source.Result),  //AndarBahar
                _             => GetBaccaratGameHistory(source.Result)     //百家樂相關
            };

        private string GetRedBlueDuelGameHistory(string[] values)
        {
            var split = values[0].Split(',');
            return JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"red", split[0]},
                    {"blue", split[1]}
                });
        }

        private string GetSicboGameHistory(string[] values)
        {
            var split = values[0].Split(',');
            return JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"dice1", split[0]},
                    {"dice2", split[1]},
                    {"dice3", split[2]}
                });
        }

        private string GetDragontigerGameHistory(string[] values) =>
            JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"dragon", _pokerMapper.Where(x => x.Key == values[0]).First().Value},
                    {"tiger", _pokerMapper.Where(x => x.Key == values[3]).First().Value}
                });

        private string GetBaccaratGameHistory(string[] values) =>
            JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {
                        "player", string.Join(',', _pokerMapper.Where(x => x.Key == values[0]).First().Value,
                            _pokerMapper.Where(x => x.Key == values[1]).First().Value,
                            _pokerMapper.Where(x => x.Key == values[2]).First().Value)
                    },
                    {
                        "banker", string.Join(',', _pokerMapper.Where(x => x.Key == values[3]).First().Value,
                            _pokerMapper.Where(x => x.Key == values[4]).First().Value,
                            _pokerMapper.Where(x => x.Key == values[5]).First().Value)
                    }
                });

        private void InitoPoker()
        {
            _pokerMapper = new Dictionary<string, string>();
            _pokerMapper.Add(string.Empty, 0.ToString("d2"));
            //黑桃= S, 愛心 = H, 方塊 = D, 梅花 = C;
            foreach (var c in new[]
            {
                "S",
                "H",
                "D",
                "C"
            })
            {
                for (var number = 1; number <= 13; number++)
                {
                    _pokerMapper.Add($"{c}{number.ToString("d2")}", $"{GetCard(c)}{GetNumber(number)}");
                }
            }
        }

        private string GetCard(string cardIndex) => cardIndex switch
        {
            "H" => "♥",
            "C" => "♣",
            "D" => "♦",
            _   => "♠"
        };

        private string GetNumber(int value) => value switch
        {
            1  => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _  => value.ToString()
        };
    }
}