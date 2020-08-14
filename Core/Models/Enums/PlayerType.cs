using System;
using System.ComponentModel;

namespace TGS.Core.Models.Enums
{
    [Flags]
    public enum PlayerTypes : byte
    {
        None = 0,
        [Description("Real Player")]
        RealPlayer = 1,
        [Description("Operator Test Player")]
        OperatorTestPlayer = 2,
        [Description("UGS Test Player")]
        UgsTestPlayer = 4,
        ExcludeUgsTestPlayer = RealPlayer | OperatorTestPlayer,
        TestPlayers = OperatorTestPlayer | UgsTestPlayer,
        All = RealPlayer | OperatorTestPlayer | UgsTestPlayer
    }
}