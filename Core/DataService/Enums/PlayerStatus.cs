using System;
using System.ComponentModel;

namespace TGS.Core.DataService.Enums
{
    [Flags]
    public enum PlayerStatus
    {
        Enabled = 0,

        Disabled = 1 << 0,//1

        [Description("Fund-In")]
        DisableFundIn = 1 << 1,//2

        [Description("Fund-Out")]
        DisableFundOut = 1 << 2,//4

        [Description("Place Bet")]
        DisablePlaceBet = 1 << 3,//8

        [Description("Transfer Of Bonus Spin Winnings")]
        DisableTransferOfBonusSpin = 1 << 4,//16

        [Description("Use of Bonus Spins")]
        DisableBonusSpinAccess = 1 << 5,//32

        DisableLobbyAccess = 1 << 6,//64

        DisableGameAccess = DisablePlaceBet | DisableBonusSpinAccess,//40

        [Description("Lobby and Game Access")]
        DisableLobbyAndGameAccess = DisablePlaceBet | DisableBonusSpinAccess | DisableLobbyAccess//104
    }
}