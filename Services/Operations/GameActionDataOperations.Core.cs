using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Bet;
using TGS.Core.Models.Services.Games;
using TGS.Infrastructures.Log;
using TGS.Services.Operations.Models;

namespace TGS.Services.Operations
{
    public sealed partial class GameActionDataOperations : IGameActionDataOperations
    {
        private async Task<BetResult> ExecuteGameActionAsync(
            BetData data,
            Func<TransactionData, TransactionSettings> getTransactionSettings)
        {
            return null;
        }
    }
}
