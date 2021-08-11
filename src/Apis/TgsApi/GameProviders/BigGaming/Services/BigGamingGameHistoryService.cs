using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.BigGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingGameHistoryService
    {
        string GetGameHistory(BigGamingVideoRoundQueryItemResponse source);
    }
    public record BigGamingGameHistoryService : IBigGamingGameHistoryService
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private Dictionary<int, string> pokerMapper;

        public BigGamingGameHistoryService(IJsonSerializationProvider jsonSerializationProvider)
        {
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            InitoPoker();
        }

        string IBigGamingGameHistoryService.GetGameHistory(BigGamingVideoRoundQueryItemResponse source) => source.GameType switch
        {
            2 => source.Result,                                                                //轮盘
            3 => GetSicboGameHistory(Convert.ToInt32(source.Result)),                          //骰宝
            4 => GetDragontigerGameHistory(Convert.ToInt32(source.Result)),                    //龙虎
            5 => source.Baccarat64,                                                            //红黑
            6 => source.Result,                                                                //极速骰宝
            8 => source.Result,                                                                //色碟
            11 => GetFullColorBaccaratGameHistory(Convert.FromBase64String(source.Baccarat64)), //多彩百家乐
            13 => GetBullBullGameHistory(Convert.FromBase64String(source.Baccarat64)),          //牛牛
            14 => GetWinThreeCardsGameHistory(Convert.FromBase64String(source.Baccarat64)),     //炸金花
            15 => source.Result,                                                                //赌场战争
            _ => GetBaccaratGameHistory(Convert.FromBase64String(source.Baccarat64))           //百家樂相關
        };

        private string GetSicboGameHistory(int value) => _jsonSerializationProvider.SerializeToString(
            new Dictionary<string, int>
            {
                {"dice1", value >> 16},
                {"dice2", (value & 0xFF00) >> 8},
                {"dice3", value & 0xFF}
            });

        private string GetDragontigerGameHistory(int value) => _jsonSerializationProvider.SerializeToString(
            new Dictionary<string, long>
            {
                {"dragon", value & 0xFFFF},
                {"tiger", (value & 0xFFFF0000) >> 16}
            });

        private string GetBaccaratGameHistory(byte[] values)
        {
            var banker = new string[3]
            {
                pokerMapper.Where(x => x.Key == values[1]).First().Value,
                pokerMapper.Where(x => x.Key == values[3]).First().Value,
                pokerMapper.Where(x => x.Key == values[5]).First().Value
            };
            var player = new string[3]
            {
                pokerMapper.Where(x => x.Key == values[0]).First().Value,
                pokerMapper.Where(x => x.Key == values[2]).First().Value,
                pokerMapper.Where(x => x.Key == values[4]).First().Value
            };

            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"player", string.Join(',', player)},
                    {"banker", string.Join(',', banker)}
                });
        }

        private string GetBullBullGameHistory(byte[] values)
        {
            var banker = new string[5];
            var player1 = new string[5];
            var player2 = new string[5];
            var player3 = new string[5];
            for (var i = 0; i < 5; i++)
            {
                var bankerIndex = i;
                var player1Index = bankerIndex + 4 + 1;
                var player2Index = player1Index + 4 + 1;
                var player3Index = player2Index + 4 + 1;
                banker[i] = pokerMapper.Where(x => x.Key == values[bankerIndex]).First().Value;
                player1[i] = pokerMapper.Where(x => x.Key == values[player1Index]).First().Value;
                player2[i] = pokerMapper.Where(x => x.Key == values[player2Index]).First().Value;
                player3[i] = pokerMapper.Where(x => x.Key == values[player3Index]).First().Value;
            }

            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"banker", string.Join(',', banker)},
                    {"player1", string.Join(',', player1)},
                    {"player2", string.Join(',', player2)},
                    {"player3", string.Join(',', player3)}
                });
        }

        private string GetWinThreeCardsGameHistory(byte[] values)
        {
            var dragon = new string[3]
            {
                pokerMapper.Where(x => x.Key == values[0]).First().Value,
                pokerMapper.Where(x => x.Key == values[1]).First().Value,
                pokerMapper.Where(x => x.Key == values[2]).First().Value
            };
            var phoenix = new string[3]
            {
                pokerMapper.Where(x => x.Key == values[3]).First().Value,
                pokerMapper.Where(x => x.Key == values[4]).First().Value,
                pokerMapper.Where(x => x.Key == values[5]).First().Value
            };

            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"dragon", string.Join(',', dragon)},
                    {"phoenix", string.Join(',', phoenix)}
                });
        }

        private string GetFullColorBaccaratGameHistory(byte[] values)
        {
            var banker = new (string Key, int Value)[3];
            var bankerIndex = 0;
            var player = new (string Key, int Value)[3];
            var playerIndex = 0;
            for (var i = 0; i < values.Length; i++)
            {
                var ColorString = values[i] == 0 ? "" : GetFullColor(values[i]);
                var NumberString = values[i] == 0 ? 0 : values[i] % 11 == 0 ? 11 : values[i] % 11;
                if (i is 1 or 3 or 5)
                {
                    banker[bankerIndex] = (ColorString, NumberString);
                    bankerIndex++;
                }
                else
                {
                    player[playerIndex] = (ColorString, NumberString);
                    playerIndex++;
                }
            }

            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"player", string.Join(",", player.Select(kv => kv.Key + ":" + kv.Value).ToArray())},
                    {"banker", string.Join(",", banker.Select(kv => kv.Key + ":" + kv.Value).ToArray())}
                });
        }

        private void InitoPoker()
        {
            pokerMapper = new Dictionary<int, string>
            {
                { 0, "00" }
            };
            var i = 0;
            for (var card = 0; card < 4; card++)
            {
                var cardString = card switch
                {
                    1 => "♥",
                    2 => "♣",
                    3 => "♦",
                    _ => "♠"
                };
                for (var number = 0; number < 13; number++)
                {
                    var numberString = (number + 1) switch
                    {
                        1 => "A",
                        11 => "J",
                        12 => "Q",
                        13 => "K",
                        _ => (number + 1).ToString()
                    };
                    pokerMapper.Add(i + 1, cardString + numberString);
                    i++;
                }
            }
        }

        private static string GetFullColor(int value)
        {
            if (value is > 0 and < 12) return "Green";
            if (value is > 11 and < 23) return "Orange";
            if (value is > 22 and < 34) return "Purple";
            if (value is > 33 and < 45) return "Blue";
            return "White";
        }
    }
}
