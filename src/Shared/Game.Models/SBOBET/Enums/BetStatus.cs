using System.ComponentModel;

namespace TGS.Shared.Game.Models.SBOBET.Enums
{
    public enum BetStatus
    {
        [Description("running")]
        Running,

        [Description("settled")]
        Settled,

        [Description("void")]
        Void
    }
}
