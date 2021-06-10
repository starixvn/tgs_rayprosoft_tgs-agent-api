using System;

namespace TGS.Services.Operator.Services.BrandPayments
{
    public record BrandPaymentsQuery(
        Guid BrandId,
        string PlayerCode);
}
