using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record BatchTransactionData
    {
        public string PlayerCode { get; init; }
        public int IndexPerPlayer { get; set; }
        public Guid GameProviderConfigurationId { get; init; }
        public decimal Amount { get; init; }
        public decimal? Commission { get; init; }
        public string CurrencyCode { get; init; }
        public string ExternalGameId { get; init; }
        public string ExternalBetId { get; init; }
        public string ExternalRoundId { get; init; }
        public string ExternalTransactionId { get; init; }
        public string ExternalTransactionReferenceId { get; init; }
        public GameActionType GameActionType { get; init; }
        public string Description { get; init; }
        public bool? RoundClosed { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public string BetType { get; init; }
        public string RoundDetail { get; init; }
    }
}