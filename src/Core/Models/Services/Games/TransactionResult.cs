using System;

namespace TGS.Core.Models.Services.Games
{
    public record TransactionResult
    {
        public bool IsDuplicate { get; init; }
        public string ExternalTransactionId { get; init; }
        public Guid? InternalTransactionId { get; init; }
        public int GameId { get; init; }
        public Guid RoundId { get; init; }
    }
}