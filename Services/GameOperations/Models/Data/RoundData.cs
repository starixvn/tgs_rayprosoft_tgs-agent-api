using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.GameOperations.Models.Data
{
    internal record RoundData
    {
        public Guid Id { get; init; }
        public string ExternalRoundId { get; init; }
        public Guid GameProviderConfigurationId { get; init; }
        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; }
        public string GameProviderName { get; init; }
        public Guid GameId { get; init; }
        public string GameName { get; init; }
        public GameType GameType { get; init; }
        public string PlayerCode { get; init; }
        public PlayerType PlayerType { get; init; }
        public PlatformType PlatformType { get; init; }
        public Guid WalletId { get; init; }
        public string CurrencyCode { get; init; }
        public Guid ResellerId { get; init; }
        public string ResellerCode { get; init; }
        public Guid LicenseeId { get; init; }
        public string LicenseeCode { get; init; }
        public Guid BrandId { get; init; }
        public string BrandCode { get; init; }
        public DateTimeOffset CreatedOn { get; init; }
        public DateTimeOffset UpdatedOn { get; init; }
        public DateTimeOffset? ClosedOn { get; init; }
        public RoundStatus Status { get; init; }
        public DateTimeOffset ExternalCreatedOn { get; init; }
        public DateTimeOffset? ExternalClosedOn { get; init; }
        public decimal? BetAmount { get; init; }
        public decimal? Commission { get; init; }
        public decimal? TieBetAdjustment { get; init; }
        public decimal? WinLoss { get; init; }
        public string RoundDetail { get; init; }
        public bool IsReopen { get; init; }
        public bool IsClosed { get; init; }
    }
}