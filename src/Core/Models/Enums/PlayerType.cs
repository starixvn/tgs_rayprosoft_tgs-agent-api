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
        [Description("Test Player")]
        TestPlayer = 2,
        All = RealPlayer | TestPlayer
    }
}