using System;

namespace TGS.Services.Operator
{
    public record BrandInfo(
        Guid BrandId,
        string Code,
        Uri DesktopLogoUrl,
        Uri MobileLogoUrl,
        Uri LoadingUrl,
        bool SmsEnabled);
}