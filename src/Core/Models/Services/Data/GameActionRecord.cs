using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GameActionRecord
    {
        public Guid Id { get; init; }
        public string ExternalBetId { get; init; }
        public string ExternalTransactionId { get; init; }
        public string ExternalGameActionGroupId { get; init; }
        public decimal Amount { get; init; }
        public DateTimeOffset RecordedOn { get; init; }
        public GameActionType Type { get; init; }
        public Guid? ReferencedGameActionId { get; init; }
    }
}