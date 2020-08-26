using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Services.Operations.Queries
{
    public interface IWalletQueries
    {
        Task<decimal> GetBalanceAsync(Guid walletId, bool dirtyRead);
    }

    public class WalletQueries : IWalletQueries
    {
        private readonly ITgsRepository _repository;

        public WalletQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<decimal> IWalletQueries.GetBalanceAsync(Guid walletId, bool dirtyRead)
        {
            const string spName = "usp_Wallet_GetBalance";
            return _repository.SqlQuerySingleAsync<decimal>(spName, parameters: new[]
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
                    Value = dirtyRead,
                    DbType = DbType.Boolean
                }
            });
        }
    }
}
