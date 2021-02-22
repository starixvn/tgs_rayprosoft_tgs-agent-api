using System;

namespace TGS.Core.Models.Services.Games
{
    public record BatchBetResultError
    {
        public int Error { get; init; }
        public string ErrorMessage { get; init; }
        public string PlayerCode { get; init; }
        public string ExternalTransactionId { get; init; }
        public Guid? InternalTransactionId { get; init; }
        public bool IsDuplicate { get; init; }
    }
}