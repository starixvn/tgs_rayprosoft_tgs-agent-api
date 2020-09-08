using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.Operations.Models
{
    public struct TransactionSettings
    {
        public GameActionType GameActionType;
        public Func<TransactionData, Action<string, decimal>> ValidateAmount;
        public Func<TransactionData, bool> HasWalletUpdate;
        public Func<decimal, decimal> GetTransactionAmount;
        public bool MustHaveExistingRound;
        public bool MustHaveReferencedTransaction;
    }
}