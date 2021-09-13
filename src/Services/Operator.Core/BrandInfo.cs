using System;

namespace TGS.Services.Operator
{
    public record BrandInfo(
        Guid BrandId,
        Uri DesktopLogoUrl,
        Uri MobileLogoUrl,
        Uri LoadingUrl);
}