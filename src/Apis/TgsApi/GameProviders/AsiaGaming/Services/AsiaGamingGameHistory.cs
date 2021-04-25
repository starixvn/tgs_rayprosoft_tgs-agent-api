// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.AsiaGaming.Common;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public sealed class AsiaGamingGameHistory : BaseBetHistory
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _json;
        
        public AsiaGamingGameHistory(
            IRoundQueries roundQueries,
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            IJsonSerializationProvider json) : base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _settingOperations = settingOperations;
            _json = json;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<AsiaGamingConfiguration>(SettingConsts
                    .GameProviderConfigurations.AsiaGaming);

            var gameType = cachedGame.ExternalId.SplitByUnderscore()[2];
            var gameHistoryType =
                configuration.GameHistoryTypes?.FirstOrDefault(h => h.GameTypes.IgnoreCaseContains(gameType));
            var history = gameHistoryType == null ? 
                round.RoundDetail.AdditionalData : 
                GetGameHistory(gameHistoryType, round.RoundDetail.AdditionalData);
            
            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                Data = new BaseData[]
                {
                    new JsonData {Value = history}
                }
            };
        }

        private string GetGameHistory(AsiaGamingGameHistoryType gameHistoryType, string roundDetail)
        {
            var gameHistory = _json.DeserializeFromString<GameHistory>(roundDetail);
            var datas = gameHistory.RoundResult.SplitByColon();
            
            var historyData = new Dictionary<string, string>();
            foreach (var data in datas)
            {
                var dataSplitBySemicolon = data.SplitBySemicolon();
                var type = dataSplitBySemicolon[0];
                var cards = GetCards(dataSplitBySemicolon.Skip(1).ToArray());
                historyData.Add(gameHistoryType.Descriptions[type], cards);
            }

            return _json.SerializeToString(historyData);
        }

        private string GetCards(string[] details)
        {
            for (var i = 0; i < details.Length; i++)
            {
                var detail = details[i];
                details[i] = GetCard(detail);
            }

            return details.JoinAsStringBySemicolon();
        }
        
        private string GetCard(string cardCode)
        {
            if (cardCode.Length < 2)
            {
                return cardCode;
            }

            var agCode = cardCode[0].ToString();
            var code = agCode switch
            {
                "C" => "♣",
                "D" => "♦",
                "H" => "♥",
                _ => "♠",
            };

            return cardCode.Replace(agCode, code);
        }
    }
}