// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.IMESports.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Services
{
    public interface IIMESportsServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, PlatformType platformType);
        
        ValueTask<IDictionary<string, IEnumerable<GameActionExternalIds>>> GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            TransactionRequest[] transactionRequests);
    }

    public sealed class IMESportsServices : IIMESportsServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;

        public IMESportsServices(
            IGameActionQueries gameActionQueries,
            IGameProviderOperations gameProviderOperations)
        {
            _gameActionQueries = gameActionQueries;
            _gameProviderOperations = gameProviderOperations;
        }

        async ValueTask<GameProviderConfigurationData> IIMESportsServices.GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
        {
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }
        
        async ValueTask<IDictionary<string, IEnumerable<GameActionExternalIds>>> IIMESportsServices.GetValidGameActionExternalIdsAsync(
            Guid gameProviderId, TransactionRequest[] transactionRequests)

        {
            if (!transactionRequests.Any())
            {
                throw new InvalidOperationException("TransactionRequest[] cannot be empty");
            }
            
            var actionTypes = new[] {PlaceBet, WinBet, LoseBet, CancelTransaction};
            var externalGameId = transactionRequests[0].ExternalGameId;
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);

            var results = new Dictionary<string, IEnumerable<GameActionExternalIds>>();
            foreach (var tran in transactionRequests)
            {
                var externalIds = await _gameActionQueries
                    .GetGameActionExternalIdsAsync(gameProviderId,
                        tran.PlayerCode, tran.ExternalRoundId, cachedGames[0].Id, actionTypes).ConfigureAwait(false);
                results.Add(tran.ExternalRoundId, GetValidGameActions(externalIds));
            }
            
            return results;
        }

        private IEnumerable<GameActionExternalIds> GetValidGameActions(GameActionExternalIds[] externalIds)
        {
            var cancelledIds = externalIds.Where(e => e.GameActionType == CancelTransaction)
                .Select(e => e.ExternalReferencedTransactionId);
            var uncancelledIds = externalIds.Where(e => e.GameActionType == CancelTransaction && 
                                                        cancelledIds.IgnoreCaseContains(e.ExternalTransactionId))
                .Select(e => e.ExternalReferencedTransactionId);
            return externalIds.Where(e => cancelledIds.IgnoreCaseNotContains(e.ExternalTransactionId) ||
                                          uncancelledIds.IgnoreCaseContains(e.ExternalTransactionId));
        }
    }
}