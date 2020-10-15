using System.ComponentModel;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Enums
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
