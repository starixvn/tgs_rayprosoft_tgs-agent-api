using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GameActionExternalIds
    {
        public string ExternalBetId { get; init; }
        public string ExternalTransactionId { get; init; }
        public string ExternalRoundId { get; init; }
        public string ExternalGameActionGroupId { get; init; }
        public string ExternalReferencedTransactionId { get; init; }
        public decimal Amount { get; init; }
        public DateTimeOffset RecordedOn { get; init; }
        public GameActionType GameActionType { get; init; }
    }
}
