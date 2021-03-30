using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Extensions
{
    public static class PlatformExtensions
    {
        public static GamePlatformType[] ToGamePlatformTypes(this PlatformType platformType) =>
            platformType switch
            {
                PlatformType.Desktop => new[] {GamePlatformType.Desktop, GamePlatformType.DesktopAndMobile},
                PlatformType.Mobile  => new[] {GamePlatformType.Mobile, GamePlatformType.DesktopAndMobile},
                _                    => throw new ArgumentException("Invalid platform type", nameof(platformType))
            };
    }
}