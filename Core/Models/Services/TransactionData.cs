using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services
{
    public class TransactionData
    {
        public decimal Amount { get; set; }
        public decimal? Commission { get; set; }
        public string CurrencyCode { get; set; }
        public string ExternalBetId { get; set; }
        public string ExternalGameId { get; set; }
        public string ExternalRoundId { get; set; }
        public string ExternalTransactionId { get; set; }
        public string ExternalTransactionReferenceId { get; set; }
        public string ExternalGameActionGroupId { get; set; }
        public GameActionType GameActionType { get; set; }
        public string Description { get; set; }
        //public string AdditionalData { get; set; }
        public bool? RoundClosed { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string BetType { get; set; }
    }
}