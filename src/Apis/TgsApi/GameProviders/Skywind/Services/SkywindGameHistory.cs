using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.Skywind;
using TGS.Shared.Game.Models.Skywind.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public sealed class SkywindGameHistory : BaseBetHistory
    {
        private readonly ISkywindApiClient _client;
        private readonly ISettingOperations _settingOperations;

        public SkywindGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ISkywindApiClient client,
            ISettingOperations settingOperations
        ) : base(roundQueries, gameProviderOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var roundId = round.ExternalRoundId;
            if (roundId.IndexOf("_") != -1)
            {
                roundId = roundId.Split("_")[0];
            }

            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<SkywindConfiguration>(SettingConsts.GameProviderConfigurations.Skywind)
                .ConfigureAwait(false);

            var response = await _client
                .GetGamHistoryRoundDetailsAsync(configuration,
                    new SkywindGamHistoryRoundDetailsRequest {RoundId = roundId}).ConfigureAwait(false);

            return new RoundResult
            {
                BetAmount = round.BetAmount ?? 0,
                Turnover = round.Turnover ?? 0,
                Winloss = round.WinLoss ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new UrlData {Url = response.ImageUrl}
                }
            };
        }
    }
}