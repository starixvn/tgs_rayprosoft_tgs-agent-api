using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.JDB;
using TGS.Shared.Game.Models.JDB.Outgoing;
using JDBGameType = TGS.Shared.Game.Models.JDB.Enums.GameType;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public sealed class JDBBetHistory : BaseBetHistory
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IJDBApiClient _jdbApiClient;
        private readonly JDBGameTypeMapper _jdbGameTypeMapper;
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IJsonSerializationProvider _json;

        public JDBBetHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            ISettingOperations settingOperations,
            IJDBApiClient jdbApiClient,
            JDBGameTypeMapper jdbGameTypeMapper,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJsonSerializationProvider json) :
            base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _settingOperations = settingOperations;
            _jdbApiClient = jdbApiClient;
            _jdbGameTypeMapper = jdbGameTypeMapper;
            _brandGameProviderOperations = brandGameProviderOperations;
            _json = json;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts.GameProviderConfigurations.JDB);
            
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(cachedGameProvider.Id, round.BrandId,
                language);

            var jdbGameType = _jdbGameTypeMapper.Get(cachedGame.Type);

            var additionalData = _json.DeserializeFromString<JDBRoundDetail>(round.RoundDetail.AdditionalData);
            var value = new BetHistoryRequest
            {
                Timestamp = DateTime.Now.Ticks,
                Parent = configuration.MerchantCode,
                UserId = playerCode,
                Language = cultureCode,
                GameType = (int)jdbGameType,
                SequenceNo = additionalData.GameSequenceNo
            };

            var response = await
                _jdbApiClient.GetBetHistoryUrlAsync(configuration.ApiUrl, configuration.ClientDomain,
                    value);
            var url = string.Empty;
            if (response.Status == "0000")
            {
                url = response.Data.FirstOrDefault()?.Path;
            }

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Winloss = (round.WinLoss ?? 0).Negate(),
                Turnover = round.Turnover ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[] {new UrlData {Url = url}}
            };
        }

        private class JDBRoundDetail
        {
            public long GameSequenceNo { get; set; }
        }
    }
}
