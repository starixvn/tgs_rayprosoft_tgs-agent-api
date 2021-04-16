using System;

namespace TGS.Services.Operator
{
    public record BrandPaymentAcl(
        Guid BrandPaymentId,
        string PlayerCode,
        bool IsBlock);
}
