using System;

namespace TGS.Core.Models.Enums
{
    [Flags]
    public enum GameTag : byte
    {
        None = 0,
        New = 1,
        Hot = 2,
        Recommend = 4
    }
}
