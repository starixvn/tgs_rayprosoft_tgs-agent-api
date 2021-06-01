using System;
using TGS.Services.Operator;

namespace TGS.Services.Operator.BrandPayment
{
    public record BrandPaymentVip(
        Guid BrandPaymentId,
        Guid VipLevelId);
}
