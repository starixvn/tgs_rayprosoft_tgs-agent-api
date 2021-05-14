using System.Text.Json;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public sealed class KYGameHistory : BaseBetHistory
    {
        public KYGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations
            ) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var gameRecord = JsonSerializer.Deserialize<KYGameRecord>(round.RoundDetail.AdditionalData);

            if (gameRecord == null)
            {
                return null;
            }


            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
               {
                    new JsonData {Value = gameRecord.CardValue}
               }
            };

        }

        private string GetData(CachedGame cachedGame, KYGameRecord gameRecord)
        {
            string data = string.Empty;
            switch (cachedGame.Type)
            {
                case GameType.CardGame:
                    data = GetCardGame(cachedGame, gameRecord.CardValue);
                    break;

            }

            return data;
        }

        private string GetCardGame(CachedGame cachedGame, string value)
        {
            return string.Empty;
        }


        private string GetMahjong(string mahjong) =>
             mahjong.ToLower() switch
             {
                 "1" => "⼀筒",
                 "2" => "⼆筒",
                 "3" => "三筒",
                 "4" => "四筒",
                 "5" => "五筒",
                 "6" => "六筒",
                 "7" => "七筒",
                 "8" => "⼋筒",
                 "9" => "九筒",
                 _ => "⽩板",
             };


        private string GetPokerBrandColor(string brandColor) =>
            brandColor.ToLower() switch
            {
                "0" => "♦",
                "1" => "♣",
                "2" => "♥",
                _ => "♠",
            };


        private string GetPokerNumbers(string number) =>
             number.ToLower() switch
             {
                 "a" => "10",
                 "b" => "J",
                 "c" => "Q",
                 "d" => "K",
                 _ => number,
             };

    }
}
