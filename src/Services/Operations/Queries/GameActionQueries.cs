using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.CustomQueryParameters;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.Operations.Queries
{
    public interface IGameActionQueries
    {
        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, bool onlyPlacements);

        Task<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId, GameActionType[] gameActionTypes);

        Task<GameActionData> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, int gameId);

        Task<bool> ReferencedGameActionExistsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, string externalTransactionId);
    }

    public sealed class GameActionQueries : IGameActionQueries
    {
        private readonly ITgsRepository _repository;

        public GameActionQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        async Task<GameActionExternalIds[]> IGameActionQueries.GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, bool onlyPlacements)
        {
            const string spName = "bet.game_action_get_valid_game_action_external_ids";
            var r = await _repository.SqlQueryMultipleAsync<GameActionExternalIds>(spName,
                parameters: new Dictionary<string, object>
                {
                    {":game_provider_id", gameProviderId},
                    {":player_code", playerCode},
                    {":external_round_id", externalRoundId},
                    {":game_id", gameId},
                    {":only_placements", onlyPlacements}
                });

            return r.ToArray();
        }

        async Task<GameActionExternalIds[]> IGameActionQueries.GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId, GameActionType[] gameActionTypes)
        {
            const string spName = "bet.game_action_get_game_action_external_ids";
            var r = await _repository.SqlQueryMultipleAsync<GameActionExternalIds>(spName, parameters:
                new Dictionary<string, object>
                {
                    {":game_provider_id", gameProviderId},
                    {":player_code", playerCode},
                    {":external_round_id", externalRoundId},
                    {":game_id", gameId},
                    {":game_action_types", new ArrayParameter<int>(gameActionTypes.Select(s => (int)s).ToArray())}
                });

            return r.ToArray();
        }

        async Task<GameActionData> IGameActionQueries.GetGameActionsByExternalRoundIdAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId)
        {
            const string spName = "bet.game_action_get_game_actions_by_external_round_id";
            var r = await _repository.SqlQueryMultipleAsync<GameActionResult>(spName, parameters:
                new Dictionary<string, object>
                {
                    {":game_provider_id", gameProviderId},
                    {":player_code", playerCode},
                    {":external_round_id", externalRoundId},
                    {":game_id", gameId}
                });

            if (!r.Any())
            {
                return null;
            }

            return r.ToLookup(o => new
                {
                    Id = o.RoundId,
                    o.ExternalRoundId,
                    Status = o.RoundStatus,
                    BetAmount = o.RoundBetAmount,
                    ValidBetAmount = o.RoundValidBetAmount,
                    WinLoss = o.RoundWinLoss,
                    o.Commission,
                    o.TieBetAdjustment,
                    o.PlatformType,
                    ClosedOn = o.RoundClosedOn
                })
                .Select(g => new GameActionData
                {
                    Round = new()
                    {
                        Id = g.Key.Id,
                        ExternalRoundId = g.Key.ExternalRoundId,
                        Status = g.Key.Status,
                        BetAmount = g.Key.BetAmount,
                        ValidBetAmount = g.Key.ValidBetAmount,
                        WinLoss = g.Key.WinLoss,
                        Commission = g.Key.Commission,
                        TieBetAdjustment = g.Key.TieBetAdjustment,
                        PlatformType = g.Key.PlatformType,
                        ClosedOn = g.Key.ClosedOn
                    },
                    GameActions = g.Select(gar => new GameActionRecord
                    {
                        Id = gar.Id,
                        ExternalTransactionId = gar.ExternalTransactionId,
                        ExternalBetId = gar.ExternalBetId,
                        ExternalGameActionGroupId = gar.ExternalGameActionGroupId,
                        Amount = gar.Amount,
                        Type = gar.GameActionType,
                        RecordedOn = gar.RecordedOn,
                        ReferencedGameActionId = gar.ReferencedGameActionId
                    }).ToArray()
                }).First();
        }

        Task<bool> IGameActionQueries.ReferencedGameActionExistsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId, string externalTransactionId)
        {
            const string spName = "bet.game_action_referenced_game_action_exists";
            return _repository.SqlQuerySingleAsync<bool>(spName, parameters:
                new Dictionary<string, object>
                {
                    {":game_provider_id", gameProviderId},
                    {":player_code", playerCode},
                    {":external_round_id", externalRoundId},
                    {":external_transaction_id", externalTransactionId}
                });
        }
    }
}