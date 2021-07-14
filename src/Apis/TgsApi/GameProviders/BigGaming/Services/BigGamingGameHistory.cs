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
        private Dictionary<int, string> pokerMapper;

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
            InitoPoker();
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

            2 => source.Result,//轮盘
            3 => GetSicboGameHistory(Convert.ToInt32(source.Result)),//骰宝
            4 => GetDragontigerGameHistory(Convert.ToInt32(source.Result)),//龙虎
            5 => source.Baccarat64,//红黑
            6 => source.Result,//极速骰宝
            8 => source.Result,//色碟
            11 => GetFullColorBaccaratGameHistory(Convert.FromBase64String(source.Baccarat64)),//多彩百家乐
            13 => GetBullBullGameHistory(Convert.FromBase64String(source.Baccarat64)),//牛牛
            14 => GetWinThreeCardsGameHistory(Convert.FromBase64String(source.Baccarat64)),//炸金花
            15 => source.Result,//赌场战争
            _ => GetBaccaratGameHistory(Convert.FromBase64String(source.Baccarat64))//百家樂相關
        };

        private string GetSicboGameHistory(int value)
        {
            return JsonSerializer.Serialize(
                        new Dictionary<string, int>() {
                        { "dice1",value >> 16},
                        { "dice2",(value & 0xFF00) >> 8},
                            { "dice3", value & 0xFF}
                            });
        }

        private string GetDragontigerGameHistory(int value)
        {
            return JsonSerializer.Serialize(
                        new Dictionary<string, long>() {
                        { "dragon",value&0xFFFF},
                        { "tiger", (value&0xFFFF0000)>>16},
                        });
        }

        private string GetBaccaratGameHistory(byte[] values)
        {
            string[] banker = new string[3] {
                pokerMapper.Where(x => x.Key == values[1]).First().Value,
                pokerMapper.Where(x => x.Key == values[3]).First().Value,
                pokerMapper.Where(x => x.Key == values[5]).First().Value,
            };
            string[] player = new string[3]{
                pokerMapper.Where(x => x.Key == values[0]).First().Value,
                pokerMapper.Where(x => x.Key == values[2]).First().Value,
                pokerMapper.Where(x => x.Key == values[4]).First().Value,
            };

            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "player",string.Join(',', player)},
                        { "banker",string.Join(',', banker)}
                               });
        }

        private string GetBullBullGameHistory(byte[] values)
        {
            var banker = new string[5];
            var player1 = new string[5];
            var player2 = new string[5];
            var player3 = new string[5];
            for (int i = 0; i < 5; i++)
            {
                int bankerIndex = i;
                int player1Index = bankerIndex + 4 + 1;
                int player2Index = player1Index + 4 + 1;
                int player3Index = player2Index + 4 + 1;
                banker[i] = pokerMapper.Where(x => x.Key == values[bankerIndex]).First().Value;
                player1[i] = pokerMapper.Where(x => x.Key == values[player1Index]).First().Value;
                player2[i] = pokerMapper.Where(x => x.Key == values[player2Index]).First().Value;
                player3[i] = pokerMapper.Where(x => x.Key == values[player3Index]).First().Value;
            }

            return JsonSerializer.Serialize(
                new Dictionary<string, string>() {
                    { "banker",string.Join(',', banker)},
                    { "player1",string.Join(',', player1)},
                    { "player2",string.Join(',', player2)},
                    { "player3",string.Join(',', player3)}
                });
        }

        private string GetWinThreeCardsGameHistory(byte[] values)
        {
            string[] dragon = new string[3] {
                pokerMapper.Where(x => x.Key == values[0]).First().Value,
                pokerMapper.Where(x => x.Key == values[1]).First().Value,
                pokerMapper.Where(x => x.Key == values[2]).First().Value,
            };
            string[] phoenix = new string[3]{
                pokerMapper.Where(x => x.Key == values[3]).First().Value,
                pokerMapper.Where(x => x.Key == values[4]).First().Value,
                pokerMapper.Where(x => x.Key == values[5]).First().Value,
            };

            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "dragon",string.Join(',', dragon)},
                        { "phoenix",string.Join(',', phoenix)}
                               });
        }

        private string GetFullColorBaccaratGameHistory(byte[] values)
        {
            var banker = new (string Key, int Value)[3];
            var bankerIndex = 0;
            var player = new (string Key, int Value)[3];
            var playerIndex = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var ColorString = (values[i] == 0) ? "" : GetFullColor(values[i]);
                var NumberString = (values[i] == 0) ? 0 : (values[i] % 11 == 0) ? 11 : values[i] % 11;
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

            return JsonSerializer.Serialize(
                           new Dictionary<string, string>() {
                        { "player",string.Join(",", player.Select(kv => kv.Key + ":" + kv.Value).ToArray())},
                        { "banker",string.Join(",", banker.Select(kv => kv.Key + ":" + kv.Value).ToArray())}
                           });
        }

        private void InitoPoker()
        {
            pokerMapper = new Dictionary<int, string>();
            pokerMapper.Add(0, "00");
            int i = 0;
            for (int card = 0; card < 4; card++)
            {
                var cardString = GetCard(card);
                for (int number = 0; number < 13; number++)
                {
                    string numberString = GetNumber(number + 1);
                    pokerMapper.Add(i + 1, cardString + numberString);
                    i++;
                }
            }

        }

        private string GetCard(int cardIndex) => cardIndex switch
        {
            1 => "♥",
            2 => "♣",
            3 => "♦",
            _ => "♠"
        };

        private string GetNumber(int value) => value switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => value.ToString()
        };

        private string GetFullColor(int value)
        {
            if (0 < value && value < 12) return "Green";
            else if (11 < value && value < 23) return "Orange";
            else if (22 < value && value < 34) return "Purple";
            else if (33 < value && value < 45) return "Blue";
            else return "White";
        }
    }
}