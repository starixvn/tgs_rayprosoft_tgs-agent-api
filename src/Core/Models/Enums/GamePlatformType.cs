using System;

namespace TGS.Core.Models.Enums
{
    [Flags]
    public enum GamePlatformType : byte
    {
        Desktop = 1,
        Mobile = 2,
        DesktopAndMobile = Desktop | Mobile
    }
}