using System.Collections.Generic;

namespace TGS.Core.DataService.Models
{
    public class GameProvider : BaseEntity
    {
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int? PopupWidth { get; set; }
        public int? PopupHeight { get; set; }
        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Reseller> Resellers { get; set; }
        public virtual ICollection<GameProviderConfiguration> GameProviderConfigurations { get; set; }
        public bool AutoCloseRound { get; set; }
        public bool SupportTestPlayer { get; set; }
        public bool SupportBetHistory { get; set; }
        public bool SupportMultipleGamesInLobby { get; set; }
        public bool SupportMultipleGamesInDirectGameLauncher { get; set; }
        public bool IomCompliant { get; set; }
        public bool SupportIFrameDesktop { get; set; }
        public bool SupportIFrameMobile { get; set; }
        public bool SupportDemoMode { get; set; }
        public int PlaceBetTimeout { get; set; }
        public int SettleBetTimeout { get; set; }
    }
}