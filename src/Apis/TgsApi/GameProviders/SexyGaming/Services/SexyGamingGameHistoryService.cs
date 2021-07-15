using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingGameHistoryService
    {
        string GetGameHistory(string externalGameId, SexyGamingGameRecord source);
    }
    public sealed class SexyGamingGameHistoryService : ISexyGamingGameHistoryService
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private Dictionary<string, string> _pokerMapper;

        public SexyGamingGameHistoryService(
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            InitoPoker();
        }

        string ISexyGamingGameHistoryService.GetGameHistory(string externalGameId, SexyGamingGameRecord source)
          => externalGameId switch
          {
              "MX-LIVE-001" => GetBaccaratGameHistory(source),    //Baccarat Classic
              "MX-LIVE-002" => GetBaccaratGameHistory(source),    //Baccarat
              "MX-LIVE-003" => GetBaccaratGameHistory(source),    //Baccarat Insurance
              "MX-LIVE-006" => GetDragontigerGameHistory(source), //DragonTiger
              "MX-LIVE-007" => GetSicboGameHistory(source),       //SicBo
              "MX-LIVE-009" => _jsonSerializationProvider.SerializeToString(source.Result),  //Roulette
              "MX-LIVE-010" => GetRedBlueDuelGameHistory(source), //Red Blue Duel
              "MX-LIVE-011" => _jsonSerializationProvider.SerializeToString(source.Result),  //Twenty
              "MX-LIVE-012" => _jsonSerializationProvider.SerializeToString(source.Result),  //AndarBahar
              _ => GetBaccaratGameHistory(source)     //百家樂相關
          };

        private string GetRedBlueDuelGameHistory(SexyGamingGameRecord source)
        {
            var split = source.Result[0].Split(',');
            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"red", split[0]},
                    {"blue", split[1]},
                    {"outcome",source.Winner}
                });
        }

        private string GetSicboGameHistory(SexyGamingGameRecord source)
        {
            var split = source.Result[0].Split(',');
            return _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"dice1", split[0]},
                    {"dice2", split[1]},
                    {"dice3", split[2]},
                    {"outcome",source.Winner}
                });
        }

        private string GetDragontigerGameHistory(SexyGamingGameRecord source) =>
            _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {"dragon", _pokerMapper.Where(x => x.Key == source.Result[0]).First().Value},
                    {"tiger", _pokerMapper.Where(x => x.Key == source.Result[3]).First().Value},
                    {"outcome",source.Winner}
                });

        private string GetBaccaratGameHistory(SexyGamingGameRecord source) =>
            _jsonSerializationProvider.SerializeToString(
                new Dictionary<string, string>
                {
                    {
                        "player", string.Join(',', _pokerMapper.Where(x => x.Key == source.Result[0]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[1]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[2]).First().Value)
                    },
                    {
                        "banker", string.Join(',', _pokerMapper.Where(x => x.Key ==  source.Result[3]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[4]).First().Value,
                            _pokerMapper.Where(x => x.Key ==  source.Result[5]).First().Value)
                    },
                    {"outcome",source.Winner}
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
    }
}
