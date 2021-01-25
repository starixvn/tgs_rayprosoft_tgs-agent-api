using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Round
    {
        public Guid Id { get; set; }
        public string ExternalRoundId { get; set; }
        public Guid WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        public int GameId { get; set; }
        public virtual Game Game { get; set; }
        public Guid GameProviderId { get; set; }
        public virtual GameProvider GameProvider { get; set; }
        public string PlayerCode { get; set; }
        public virtual Player Player { get; set; }
        public string CurrencyCode { get; set; }
        public PlatformType PlatformType { get; set; }
        public RoundStatus Status { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public virtual GameProviderConfiguration GameProviderConfiguration { get; set; }
        public virtual ICollection<GameAction> GameActions { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ClosedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public DateTimeOffset? ExternalCreatedOn { get; set; }
        public DateTimeOffset? ExternalClosedOn { get; set; }
        public decimal? BetAmount { get; set; }
        public decimal? Commission { get; set; }
        public decimal? WinLoss { get; set; }
        public decimal? TieBetAdjustment { get; set; }
        public virtual RoundDetail RoundDetail { get; set; }
    }
}