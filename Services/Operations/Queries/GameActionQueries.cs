using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Bet;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.Operations.Queries
{
    public interface IGameActionQueries
    {
        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, bool onlyPlacements);

        Task<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, GameActionType[] gameActionTypes);

        Task<GameActionData> GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId);
    }

    public sealed class GameActionQueries : IGameActionQueries
    {
        private readonly IBetRepository _repository;

        public GameActionQueries(IBetRepository repository)
        {
            _repository = repository;
        }

        async Task<GameActionExternalIds[]> IGameActionQueries.GetValidGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, bool onlyPlacements)
        {
            const string spName = "usp_GameAction_GetValidGameActionExternalIds";
            var r = await _repository.SqlQueryMultipleAsync<GameActionExternalIds>(spName, parameters:
            new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameProviderId",
                    Value = gameProviderId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@externalRoundId",
                    Value = externalRoundId,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@onlyPlacements",
                    Value = onlyPlacements,
                    DbType = DbType.Boolean
                }
            });

            return r.ToArray();
        }

        async Task<GameActionExternalIds[]> IGameActionQueries.GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode, string externalRoundId, GameActionType[] gameActionTypes)
        {
            const string spName = "usp_GameAction_GetGameActionExternalIds";
            var r = await _repository.SqlQueryMultipleAsync<GameActionExternalIds>(spName, parameters:
            new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameProviderId",
                    Value = gameProviderId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@externalRoundId",
                    Value = externalRoundId,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@gameActionTypes",
                    Value = gameActionTypes.Select(s => new { Value = (int)s }).ToDataTable(),
                    SqlDbType = SqlDbType.Structured,
                }
            });

            return r.ToArray();
        }

        async Task<GameActionData> IGameActionQueries.GetGameActionsByExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalRoundId)
        {
            const string spName = "usp_GameAction_GetGameActionsByExternalRoundId";
            var r = await _repository.SqlQueryMultipleAsync<GameActionResult>(spName, parameters:
            new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameProviderId",
                    Value = gameProviderId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@playerCode",
                    Value = playerCode,
                    DbType = DbType.String
                },
                new()
                {
                    ParameterName = "@externalRoundId",
                    Value = externalRoundId,
                    DbType = DbType.String
                }
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
                WinLoss = o.RoundWinLoss,
                o.Commission,
                o.TieBetAdjustment,
                o.PlatformType,
                ClosedOn = o.RoundClosedOn
            })
            .Select(g => new GameActionData
            {
                Round = new RoundRecord 
                {
                    Id = g.Key.Id,
                    ExternalRoundId = g.Key.ExternalRoundId,
                    Status = g.Key.Status,
                    BetAmount = g.Key.BetAmount,
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
    }
}