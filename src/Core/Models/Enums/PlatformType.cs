using System;

namespace TGS.Core.Models.Enums
{
    /// <summary>
    ///     Platform Type
    /// </summary>
    [Flags]
    public enum PlatformType
    {
        /// <summary>
        ///     Desktop
        /// </summary>
        Desktop = 0,

        /// <summary>
        ///     Mobile
        /// </summary>
        Mobile = 1,

        /// <summary>
        ///     Desktop & Mobile
        /// </summary>
        DesktopAndMobile = Desktop | Mobile
    }
}