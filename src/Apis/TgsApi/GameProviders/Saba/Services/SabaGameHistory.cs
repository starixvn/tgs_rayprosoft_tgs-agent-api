using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.BetHistory;
using TGS.Apis.TgsApi.Models.BetHistory;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.Saba;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public sealed class SabaGameHistory : BaseBetHistory
    {
        private readonly ISabaApiClient _client;

        public SabaGameHistory(
            IRoundQueries roundQueries,
            IGameProviderOperations gameProviderOperations,
            ILocalizationOperations localizationOperations,
            ISabaApiClient client) :
            base(roundQueries, gameProviderOperations, localizationOperations)
        {
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

            return await ValueTask.FromResult(new RoundResult());
        }
    }
}