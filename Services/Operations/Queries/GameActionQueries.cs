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

namespace TGS.Services.Operations.Queries
{
    public interface IGameActionQueries
    {
        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId, Guid walletId, string externalRoundId, bool onlyPlacements);

        Task<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, Guid walletId, string externalRoundId, GameActionType[] gameActionTypes);
    }

    public sealed class GameActionQueries : IGameActionQueries
    {
        private readonly IBetRepository _repository;

        public GameActionQueries(IBetRepository repository)
        {
            _repository = repository;
        }

        async Task<GameActionExternalIds[]> IGameActionQueries.GetValidGameActionExternalIdsAsync(Guid gameProviderId, Guid walletId, string externalRoundId, bool onlyPlacements)
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
                    ParameterName = "@walletId",
                    Value = walletId,
                    DbType = DbType.Guid
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

        async Task<GameActionExternalIds[]> IGameActionQueries.GetGameActionExternalIdsAsync(Guid gameProviderId, Guid walletId, string externalRoundId, GameActionType[] gameActionTypes)
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
                    ParameterName = "@walletId",
                    Value = walletId,
                    DbType = DbType.Guid
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
    }
}