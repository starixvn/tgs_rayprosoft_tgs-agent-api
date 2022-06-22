using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Games;

public record BetData
{
    public bool AllowCancelCancelTransaction { get; init; }
    public bool AllowPlaceBetWithDifferentPlatformToken { get; init; }
    public bool AutoCloseRound { get; init; }
    public Guid? BatchId { get; init; }
    public string CurrencyCode { get; init; }
    public bool ForceGetPreviousActions { get; init; }
    public bool FreeBetAsSingleRound { get; init; }
    public string GameProviderCode { get; init; }
    public Guid GameProviderConfigurationId { get; init; }
    public Guid GameProviderId { get; init; }
    public bool HasCurrencyFromGameProvider { get; init; } = true;
    public bool IgnorePlaceBetValidation { get; init; } = false;
    public OppositeBetRule[] OppositeBetRules { get; init; }
    public PlatformType? PlatformType { get; init; }
    public string PlayerCode { get; init; }
    public TransactionData[] Transactions { get; init; }
    public bool WithCustomBetValues { get; init; } = false;
}