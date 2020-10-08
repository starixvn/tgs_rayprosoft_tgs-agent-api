using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private static Task<RoundIdAndStatusData> GetRoundByExternalRoundIdAsync(Db db, Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "usp_Round_GetRoundByExternalRoundId";
            return db.Repository.SqlQuerySingleAsync<RoundIdAndStatusData>(sql, transaction: db.Transaction, parameters: new SqlParameter[]
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

        private static async Task<SameRoundActionData[]> GetGameActionsInSameRoundAsync(Db db, Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "usp_GameAction_GetGameActionsInSameRound";
            var data = await db.Repository.SqlQueryMultipleAsync<SameRoundActionData>(sql, transaction: db.Transaction, parameters: new SqlParameter[]
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

        private static Task<string> GetExternalRoundIdAsync(Db db, string externalTransactionId)
        {
            const string sql = "usp_Round_GetExternalRoundIdByExternalTransactionId";
            return db.Repository.SqlQuerySingleAsync<string>(sql, transaction: db.Transaction, parameters: new SqlParameter
            {
                ParameterName = "@externalTransactionId",
                Value = externalTransactionId,
                DbType = DbType.String
            });
        }

        private static Task<decimal> GetBalanceAsync(Db db, Guid walletId)
        {
            const string spName = "usp_Wallet_GetBalance";
            return db.Repository.SqlQuerySingleAsync<decimal>(spName, transaction: db.Transaction, parameters: new[]
            {
                new SqlParameter
                {
                    ParameterName = "@walletId",
                    Value = walletId,
                    DbType = DbType.Guid
                },
                new SqlParameter
                {
                    ParameterName = "@isDirtyRead",
                    Value = false,
                    DbType = DbType.Boolean
                }
            });
        }
    }
}