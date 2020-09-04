using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record BetData
    {
        public string PlayerCode { get; set; }

        public PlatformType? PlatformType { get; init; }

        public Guid GameProviderConfigurationId { get; init; }

        public Guid GameProviderId { get; init; }

        public string GameProviderCode { get; init; }

        public bool AutoCloseRound { get; init; }

        public TransactionData[] Transactions { get; init; }

        public bool HasCurrencyFromGameProvider { get; init; } = true;

        public bool ForceGetPreviousActions { get; init; } = false;

        //public bool AllowDuplicateTransactionIdAtDifferentRound { get; set; }
    }
}
