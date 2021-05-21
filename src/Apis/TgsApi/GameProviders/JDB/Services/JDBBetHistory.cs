using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.JDB;
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public sealed class JDBBetHistory : BaseBetHistory
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IJDBApiClient _jdbApiClient;
        private readonly IJDBServices _jdbServices;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public JDBBetHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            ISettingOperations settingOperations,
            IJDBApiClient jdbApiClient,
            IBrandGameProviderOperations brandGameProviderOperations,
            IPlayerOperations playerOperations,
            IJDBServices jdbServices) :
            base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _settingOperations = settingOperations;
            _jdbApiClient = jdbApiClient;
            _brandGameProviderOperations = brandGameProviderOperations;
            _playerOperations = playerOperations;
            _jdbServices = jdbServices;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts
                    .GameProviderConfigurations.JDB);
            var agent = _jdbServices.GetAgent(configuration, player.DefaultWalletCurrencyCode);

            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(cachedGameProvider.Id, round.BrandId,
                language);

            if (!long.TryParse(round.ExternalRoundId, out var externalRoundId))
            {
                throw new ArgumentException("Invalid external round id");
            }

            var items = cachedGame.ExternalId.Split("_");
            if (items.Length != 2 || !int.TryParse(items[0], out var gtype))
            {
                throw new ArgumentException($"Invalid external game id '{cachedGame.ExternalId}'");
            }

            var request = new BetHistoryRequest
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Parent = agent.Id,
                UserId = playerCode,
                Language = cultureCode,
                GameType = gtype,
                SequenceNo = externalRoundId
            };

            var response = await _jdbApiClient.GetBetHistoryUrlAsync(request);
            var url = string.Empty;
            if (response.Status.IgnoreCaseEquals(GameApiStatus.Succeed.Description()))
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
    }
}