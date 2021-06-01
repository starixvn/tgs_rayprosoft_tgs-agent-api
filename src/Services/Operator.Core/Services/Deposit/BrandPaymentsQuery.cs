using System;

namespace TGS.Services.Operator.Services.Deposit
{
    public record BrandPaymentsQuery(
        Guid BrandId,
        string PlayerCode);
}
