using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using TGS.Shared.Game.Models.AllBet.Common;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public class AllBetGameHistory : BaseBetHistory
    {
        private readonly IJsonSerializationProvider _json;

        public AllBetGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IJsonSerializationProvider json)
            : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _json = json;
        }

        protected override ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider,
            CachedGame cachedGame, string language)
        {
            var value = GetGameHistory(round.RoundDetail.AdditionalData);

            return ValueTask.FromResult(new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData {Value = value}
                }
            });
        }

        private string GetGameHistory(string roundDetail)
        {
            if (roundDetail.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var history = _json.DeserializeFromString<GameHistory>(roundDetail);

            return history.GameType switch
            {
                101 => GetBaccaratGameHistory(history),
                102 => GetBaccaratGameHistory(history),
                103 => GetBaccaratGameHistory(history),
                104 => GetBaccaratGameHistory(history),
                201 => GetSicboGameHistory(history),
                301 => GetDragonTigerGameHistory(history),
                401 => GetRouletteGameHistory(history),
                501 => GetPokDengGameHistory(history),
                801 => GetBullBullGameHistory(history),
                901 => GetWinThreeCardsGameHistory(history),
                _   => GetBaccaratGameHistory(history)
            };
        }


        private string GetBaccaratGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);

            var results = new Dictionary<string, string>();

            for (var i = 0; i < values.Length; i++)
            {
                var key = i == 0 ? "Player" : "Banker";
                var details = values[i].SplitByComma();
                var cards = GetCards(details);

                results[key] = cards;
            }

            return _json.SerializeToString(results);
        }

        private string GetSicboGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);
            var results = values[0].SplitByComma()
                .Select((v, i) => new
                {
                    Value = v,
                    DiceNumber = i + 1
                })
                .ToDictionary(v => $"Dice{v.DiceNumber}", v => v.Value);

            return _json.SerializeToString(results);
        }

        private string GetDragonTigerGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);

            var results = new Dictionary<string, string>();

            for (var i = 0; i < values.Length; i++)
            {
                var key = i == 0 ? "Dragon" : "Tiger";
                var cards = GetCards(new[]
                {
                    values[i]
                });

                results[key] = cards;
            }

            return _json.SerializeToString(results);
        }

        private string GetRouletteGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);

            return _json.SerializeToString(new {Result = values[0]});
        }

        private string GetPokDengGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);

            var results = new Dictionary<string, string>();

            for (var i = 0; i < values.Length; i++)
            {
                var key = i == 0 ? "Banker" : $"Player{i}";
                var details = values[i].SplitByComma();
                var cards = GetCards(details);

                results[key] = cards;
            }

            return _json.SerializeToString(results);
        }

        private string GetBullBullGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);

            var results = new Dictionary<string, string>();

            for (var i = 0; i < values.Length; i++)
            {
                var key = i switch
                {
                    0 => "First",
                    1 => "Banker",
                    _ => $"Player{i - 1}"
                };

                var details = values[i].SplitByComma();
                var cards = GetCards(details);

                results[key] = cards;
            }

            return _json.SerializeToString(results);
        }

        /// <summary>
        ///     炸金花
        /// </summary>
        private string GetWinThreeCardsGameHistory(GameHistory history)
        {
            var values = GetResultValues(history.Result);

            var results = new Dictionary<string, string>();

            for (var i = 0; i < values.Length; i++)
            {
                var key = i == 0 ? "Dragon" : "Phoenix";
                var cards = GetCards(new[]
                {
                    values[i]
                });

                results[key] = cards;
            }

            return _json.SerializeToString(results);
        }

        private static string[] GetResultValues(string result)
        {
            var matches = Regex.Matches(result, "{.+?}");
            return matches.Select(c =>
                    c.Value
                        .Replace("{", string.Empty)
                        .Replace("}", string.Empty))
                .ToArray();
        }

        private static string GetCards(string[] details)
        {
            var cards = details.Where(v => v != "-1")
                .Select(v => $"{GetCard(v[0])}{GetNumber(v[1..])}");

            return cards.JoinAsStringBySemicolon();
        }

        private static string GetCard(char code) => code switch
        {
            '2' => "♥",
            '3' => "♣",
            '4' => "♦",
            _   => "♠"
        };

        private static string GetNumber(string value)
        {
            if (!int.TryParse(value, out var num))
            {
                return string.Empty;
            }

            return num switch
            {
                11 => "J",
                12 => "Q",
                13 => "K",
                _  => num.ToString()
            };
        }
    }
}