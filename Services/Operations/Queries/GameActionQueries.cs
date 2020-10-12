using System;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Bet;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IGameActionQueries
    {
        Task<GameActionExternalIds[]> GetValidGameActionExternalIdsAsync(Guid gameProviderId, Guid walletId, string externalRoundId, bool onlyPlacements);
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
    }
}