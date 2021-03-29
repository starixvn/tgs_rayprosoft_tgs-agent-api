using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games
{
    public record BetData
    {
        public string PlayerCode { get; init; }
        public PlatformType? PlatformType { get; init; }
        public string CurrencyCode { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; }
        public bool AutoCloseRound { get; init; }
        public TransactionData[] Transactions { get; init; }
        public bool HasCurrencyFromGameProvider { get; init; } = true;
        public bool ForceGetPreviousActions { get; init; }
        public bool AllowCancelCancelTransaction { get; init; }
        public bool FreeBetAsSingleRound { get; init; }
        public bool AllowPlaceBetWithDifferentPlatformToken { get; init; }
        public Guid? BatchId { get; init; }
    }
}