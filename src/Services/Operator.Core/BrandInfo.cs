using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator;

public record BrandInfo(
    Guid BrandId,
    string Code,
    Uri DesktopLogoUrl,
    Uri MobileLogoUrl,
    Uri LoadingUrl,
    BrandStatus Status,
    string Currency,
    bool SmsEnabled,
    bool SmsRegisterFlowEnabled,
    bool PlayerIdentityEnabled,
    bool EmailRegisterFlowEnabled);
