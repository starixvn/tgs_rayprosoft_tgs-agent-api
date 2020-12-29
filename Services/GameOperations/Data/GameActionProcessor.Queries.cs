using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Services.GameOperations.Models.Data;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionProcessor
    {
        private static Task<RoundIdAndStatusData> GetRoundByExternalRoundIdAsync(ITgsRepository repository,
            Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "usp_Round_GetRoundByExternalRoundId";
            return repository.SqlQuerySingleAsync<RoundIdAndStatusData>(sql, parameters: new SqlParameter[]
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

        private static async Task<SameRoundActionData[]> GetGameActionsInSameRoundAsync(ITgsRepository repository,
            Guid gameProviderConfigurationId, Guid walletId, string externalRoundId)
        {
            const string sql = "usp_GameAction_GetGameActionsInSameRound";
            var data = await repository.SqlQueryMultipleAsync<SameRoundActionData>(sql, parameters: new SqlParameter[]
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

        private static Task<string> GetExternalRoundIdAsync(ITgsRepository repository, string externalTransactionId)
        {
            const string sql = "usp_Round_GetExternalRoundIdByExternalTransactionId";
            return repository.SqlQuerySingleAsync<string>(sql, parameters: new SqlParameter
            {
                ParameterName = "@externalTransactionId",
                Value = externalTransactionId,
                DbType = DbType.String
            });
        }

        private static Task<decimal> GetBalanceAsync(ITgsRepository repository, Guid walletId)
        {
            const string spName = "usp_Wallet_GetBalance";
            return repository.SqlQuerySingleAsync<decimal>(spName, parameters: new[]
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