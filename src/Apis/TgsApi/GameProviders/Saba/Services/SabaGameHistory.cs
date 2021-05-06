using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.BetHistories;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.Saba;
using TGS.Shared.Game.Models.Saba.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public sealed class SabaGameHistory : BaseBetHistory
    {
        private readonly ISabaApiClient _client;
        private readonly IJsonSerializationProvider _json;
        private readonly ISettingOperations _settingOperations;

        public SabaGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            ISettingOperations settingOperations,
            IJsonSerializationProvider json,
            ISabaApiClient client) :
            base(roundQueries, gameProviderOperations, localizationOperations)
        {
            _settingOperations = settingOperations;
            _json = json;
            _client = client;
        }

        protected override async ValueTask<RoundResult> GetGameProviderBetHistoryAsync(string playerCode, Round round,
            CachedGameProvider cachedGameProvider, CachedGame cachedGame, string language)
        {
            var placeBetAction = round.GameActions.FirstOrDefault(ga => ga.Type == GameActionType.PlaceBet);

            if (placeBetAction == null)
            {
                throw new InvalidGameActionException($"Data in the round with id {round.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SabaConfiguration>(SettingConsts
                    .GameProviderConfigurations.Saba);

            var response = await _client.GetBetDetailByTransIdAsync(configuration.ApiUrl,
                new()
                {
                    VendorId = configuration.VendorId,
                    ExternalTransactionId = placeBetAction.ExternalTransactionId
                });

            if (response.ErrorCode != GetRoundHistoryApiError.Ok)
            {
                throw new GameProviderApiErrorException($"Get sbo bet history fail: {response.ErrorMessage}");
            }

            var gameResults = response.Data?.Results;

            if (gameResults == null || gameResults.Length == 0)
            {
                return new();
            }

            return new()
            {
                BetAmount = round.BetAmount ?? 0,
                Winloss = (round.WinLoss ?? 0).Negate(),
                Turnover = round.Turnover ?? 0,
                GameResult = round.ClientStatus.ToString(),
                Data = new BaseData[]
                {
                    new JsonData {Value = _json.SerializeToString(gameResults, new() {WriteIndented = false})}
                }
            };
        }
    }
}