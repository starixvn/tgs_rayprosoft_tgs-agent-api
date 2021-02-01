using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Models.Round
{
    public class RoundDetailModel
    {
        public Guid Id { get; set; }
        public string ExternalRoundId { get; set; }
        public Guid WalletId { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public Guid GameProviderId { get; set; }
        public string GameProviderName { get; set; }
        public string PlayerName { get; set; }
        public string PlayerCode { get; set; }
        public string CurrencyCode { get; set; }
        public PlatformType PlatformType { get; set; }
        public RoundStatus Status { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ClosedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public Guid BrandId { get; set; }
        public DateTimeOffset? ExternalCreatedOn { get; set; }
        public DateTimeOffset? ExternalClosedOn { get; set; }
        public decimal? BetAmount { get; set; }
        public decimal? ValidBetAmount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? WinLoss { get; set; }
        public decimal? TieBetAdjustment { get; set; }
    }
}