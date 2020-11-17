using System;
using System.ComponentModel;

namespace TGS.Core.Models.Enums
{
    [Flags]
    public enum PlayerType : byte
    {
        None = 0,
        [Description("Real Player")]
        RealPlayer = 1,
        [Description("Operator Test Player")]
        OperatorTestPlayer = 2,
        [Description("TGS Test Player")]
        TgsTestPlayer = 3,
        ExcludeTgsTestPlayer = RealPlayer | OperatorTestPlayer,
        TestPlayers = OperatorTestPlayer | TgsTestPlayer,
        All = RealPlayer | OperatorTestPlayer | TgsTestPlayer
    }
}