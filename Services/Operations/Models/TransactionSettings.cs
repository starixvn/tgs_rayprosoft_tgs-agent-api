using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;

namespace TGS.Services.Operations.Models
{
    internal record TransactionSettings
    {
        public GameActionType GameActionType;
        public Func<TransactionData, Action<decimal>> ValidateAmount;
        public Func<TransactionData, bool> HasWalletUpdate;
        public Func<decimal, decimal> GetTransactionAmount;
        public bool MustHaveExistingRound;
        public bool MustHaveReferencedTransaction;
    }
}