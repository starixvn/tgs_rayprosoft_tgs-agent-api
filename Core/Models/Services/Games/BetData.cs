using System;

namespace TGS.Core.Models.Services.Games
{
    public record BetData
    {
        public Guid GameProviderConfigurationId { get; init; }
        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; }
        public bool AutoCloseRound { get; init; }
        public TransactionData[] Transactions { get; init; }
        public bool HasCurrencyFromGameProvider { get; init; } = true;
        public bool ForceGetPreviousActions { get; init; } = false;
        //public Guid? PlaceBetGameActionId { get; set; }
        //public decimal? UnlockAmount { get; set; }
        //public bool AllowDuplicateTransactionIdAtDifferentRound { get; set; }
    }
}
