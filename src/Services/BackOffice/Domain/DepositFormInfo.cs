using System;

namespace TGS.Services.BackOffice.Domain
{
    public record DepositFormInfo(
        Guid Id,
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        string BankAccount,
        decimal Amount,
        string Remark);
}
