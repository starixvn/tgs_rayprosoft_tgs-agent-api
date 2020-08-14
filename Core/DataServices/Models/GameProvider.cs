using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class GameProvider : BaseEntity
    {
        public Status Status { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<ResellerGameProvider> Resellers { get; set; }
        public virtual ICollection<GameProviderCurrency> Currencies { get; set; }
        public virtual ICollection<GameProviderLanguage> Languages { get; set; }
        public virtual ICollection<GameProviderConfiguration> GameProviderConfigurations { get; set; }
        public bool AutoCloseRound { get; set; }
        public bool SupportTestPlayer { get; set; }
        public int PlaceBetTimeout { get; set; }
        public int SettleBetTimeout { get; set; }
    }
}