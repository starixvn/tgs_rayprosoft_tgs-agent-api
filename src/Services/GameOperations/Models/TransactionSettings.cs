using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Models
{
    public struct TransactionSettings
    {
        public GameActionType GameActionType;
        public Action<string, decimal> ValidateAmount;
        public bool HasWalletUpdate;
        public Func<decimal, decimal> GetTransactionAmount;
        public bool MustHaveExistingRound;
        public bool MustHaveReferencedTransaction;
    }
}