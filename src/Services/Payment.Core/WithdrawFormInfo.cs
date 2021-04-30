using System;

namespace TGS.Services.Payment
{
    public record WithdrawFormInfo(
        string PlayerCode,
        Guid BrandId,
        decimal Amount,
        int Status,
        Guid BankInfoId,
        Guid? BrandPaymentId = null,
        string? BankInfo = null,
        string? ApplyInfo = null,
        string? PaymentReturn = null);
}
