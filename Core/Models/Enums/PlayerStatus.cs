using System;
using System.ComponentModel;

namespace TGS.Core.Models.Enums
{
    [Flags]
    public enum PlayerStatus
    {
        Enabled = 0,

        Disabled = 1 << 0,//1

        [Description("Place Bet")]
        DisablePlaceBet = 1 << 1,//2
    }
}