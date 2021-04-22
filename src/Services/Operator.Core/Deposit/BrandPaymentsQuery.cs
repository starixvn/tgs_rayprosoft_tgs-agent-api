using System;

namespace TGS.Services.Operator.Deposit
{
    public record BrandPaymentsQuery(
        Guid BrandId,
        string PlayerCode);
}
