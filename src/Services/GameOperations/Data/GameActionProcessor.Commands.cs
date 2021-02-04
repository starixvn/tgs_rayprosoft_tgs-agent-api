using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.GameOperations.Data
{
    internal sealed partial class GameActionProcessor
    {
        private static Task<decimal> AtomicUpdateBalanceAsync(ITgsRepository repository, string playerCode,
            Guid walletId, decimal amount, DateTimeOffset updatedOn)
        {
            const string sql = "dbo.wallet_update_wallet_balance";
            return repository.SqlQuerySingleAsync<decimal>(sql, parameters: new Dictionary<string, object>
            {
                {":player_code", playerCode},
                {":wallet_id", walletId},
                {":amount", amount},
                {":updated_on", updatedOn}
            });
        }
    }
}