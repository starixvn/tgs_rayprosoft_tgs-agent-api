using System;

namespace TGS.Services.Payment
{
    public record BrandPaymentAcl(
        Guid BrandPaymentId,
        string PlayerCode,
        bool IsBlock);
}
