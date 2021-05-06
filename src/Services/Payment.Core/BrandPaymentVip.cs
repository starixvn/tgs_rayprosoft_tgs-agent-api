using System;

namespace TGS.Services.Payment
{
    public record BrandPaymentVip(
        Guid BrandPaymentId,
        Guid VipLevelId);
}
