using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Bet.Models
{
    public class Round
    {
        public Guid Id { get; set; }
        public string ExternalRoundId { get; set; }
        public Guid WalletId { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public string GameCode { get; set; }
        public string GameProviderName { get; set; }
        public string GameProviderCode { get; set; }
        public RoundStatus Status { get; set; }
        public string AdditionalData { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public string GameProviderConfiguration { get; set; }
        public virtual ICollection<GameAction> GameActions { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ClosedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public Guid? BrandId { get; set; }
        public DateTimeOffset? ExternalCreatedOn { get; set; }
        public DateTimeOffset? ExternalClosedOn { get; set; }
        public DateTimeOffset? AggregatedOn { get; set; }
        public DateTimeOffset? ExternalAggregatedOn { get; set; }
        public decimal? BetAmount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? GrossGamingRevenue { get; set; }
        public decimal? TiebetAdjustment { get; set; }
        public decimal? JackpotContributionConvertedAmount { get; set; }
        public Guid? SessionId { get; set; }
    }
}
