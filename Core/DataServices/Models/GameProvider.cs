using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class GameProvider : BaseEntity<Guid>
    {
        public Status Status { get; set; }
        public string Description { get; set; }
        public bool AutoCloseRound { get; set; }
        public bool SupportTestPlayer { get; set; }
        public bool AllowKickPlayer { get; set; }
        public int PlaceBetTimeout { get; set; }
        public int SettleBetTimeout { get; set; }
        public short UniqueId { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<ResellerGameProvider> Resellers { get; set; }
        public virtual ICollection<LicenseeGameProvider> Licensees { get; set; }
        public virtual ICollection<BrandGameProvider> Brands { get; set; }
        public virtual ICollection<GameProviderCurrency> Currencies { get; set; }
        public virtual ICollection<GameProviderLanguage> Languages { get; set; }
        public virtual ICollection<GameProviderConfiguration> GameProviderConfigurations { get; set; }
        public virtual ICollection<GameProviderBetLimit> BetLimits { get; set; }
        public virtual ICollection<SportType> SportTypes { get; set; }
        public virtual ICollection<MarketType> MarketTypes { get; set; }
    }
}