using System;

namespace TGS.Services.Operations.Models.Data
{
    internal record TransactionResult
    {
        public bool IsDuplicate { get; init; }
        public string ExternalTransactionId { get; init; }
        public Guid? InternalTransactionId { get; init; }
    }
}
