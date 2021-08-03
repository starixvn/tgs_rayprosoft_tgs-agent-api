// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.SBOBET;
using TGS.Shared.Game.Models.SBOBET.Enums;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETGameHistory : BaseBetHistory
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly ISBOApiClient _client;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public SBOBETGameHistory(
            IRoundQueries roundQueries,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJsonSerializationProvider json,
            ISBOApiClient client) : base(roundQueries, gameProviderOperations)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
            _json = json;
            _client = client;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            var protfolio = Utility.GetPortfolio(cachedGame.Type);

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);
            var culture = await _brandGameProviderOperations.MapCultureAsync(cachedGameProvider.Id, brandId, language)
                .ConfigureAwait(false);

            var response =
                await _client.GetRoundHistoryAsync(configuration.ApiUrl, new()
                {
                    CompanyKey = configuration.CompanyKey,
                    Portfolio = protfolio,
                    ExternalRoundId = round.ExternalRoundId,
                    Language = culture
                }).ConfigureAwait(false);

            if (response.Error.Code != ServiceApiError.None)
            {
                throw new GameProviderApiErrorException($"Get sbo bet history fail: {response.Error.Message}");
            }

            var gameResults = response.Results;

            if (gameResults == null || gameResults.Length == 0)
            {
                return new();
            }

            var item = (from r in gameResults
                        group r by 1
                        into g
                        select new
                        {
                            BetAmount = g.Sum(x => x.ActualStake),
                            Turnover = g.Sum(x => x.Turnover),
                            Winloss = g.Sum(x => x.WinLose)
                        }).First();

            return new()
            {
                BetAmount = item.BetAmount,
                Turnover = item.Turnover,
                Winloss = item.Winloss,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData {Value = _json.SerializeToString(gameResults, new() {WriteIndented = false})}
                }
            };
        }
    }
}