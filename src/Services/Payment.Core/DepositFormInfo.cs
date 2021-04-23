using System;

namespace TGS.Services.Payment
{
    public record DepositFormInfo(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        decimal Amount,
        string? ApplyInfo,
        string? PaymentReturn = null);
}
