using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public record GameActionExternalIds
    {
        public string ExternalBetId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string ExternalRoundId { get; set; }
        public string ExternalGameActionGroupId { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
        public GameActionType GameActionType { get; set; }
    }
}
