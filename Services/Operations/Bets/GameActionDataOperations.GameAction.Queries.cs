using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.Operations.Models.Data;

namespace TGS.Services.Operations.Bets
{
    public sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private Task<RoundData> GetRoundByExternalRoundIdAsync(ITgsRepository repo, Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "usp_Round_GetRoundByExternalRoundId";
            return repo.SqlQuerySingleAsync<RoundData>(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameProviderConfigurationId",
                    Value = gameProviderConfigurationId,
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
                }
            });
        }

        private async Task<SameRoundActionData[]> GetGameActionsInSameRoundAsync(ITgsRepository repo, Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "usp_GameAction_GetGameActionsInSameRound";
            var data = await repo.SqlQueryMultipleAsync<SameRoundActionData>(sql, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameProviderConfigurationId",
                    Value = gameProviderConfigurationId,
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
                }
            });

            return data.ToArray();
        }

        private Task<string> GetExternalRoundIdAsync(ITgsRepository repo, string externalTransactionId)
        {
            const string sql = "usp_Round_GetExternalRoundIdByExternalTransactionId";
            return repo.SqlQuerySingleAsync<string>(sql, parameters: new SqlParameter
            {
                ParameterName = "@externalTransactionId",
                Value = externalTransactionId,
                DbType = DbType.String
            });
        }
    }
}