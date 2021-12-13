using System;

namespace TGS.Services.Agent
{
    public record BrandInfo(
        Guid BrandId,
        string Code,
        bool SmsEnabled,
        bool SmsRegisterFlowEnabled);
}
