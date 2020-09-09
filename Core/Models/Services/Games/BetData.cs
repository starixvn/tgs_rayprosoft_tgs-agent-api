using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record BetData
    {
        public string PlayerCode;

        public PlatformType? PlatformType;

        public Guid GameProviderConfigurationId;

        public Guid GameProviderId;

        public string GameProviderCode ;

        public bool AutoCloseRound;

        public TransactionData[] Transactions;

        public bool HasCurrencyFromGameProvider { get; init; } = true;

        public bool ForceGetPreviousActions { get; init; } = false;

        public Guid? BatchId;

        public bool FreeBetAsSingleRound { get; init; } = false;

        //public bool AllowDuplicateTransactionIdAtDifferentRound { get; set; }
    }
}
