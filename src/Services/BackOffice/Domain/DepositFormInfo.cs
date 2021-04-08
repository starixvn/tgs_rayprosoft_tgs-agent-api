using System;

namespace TGS.Services.BackOffice.Domain
{
    public record DepositFormInfo(
        Guid Id,
        string PlayerCode,
        string UserName,
        Guid BrandId,
        Guid BrandPaymentId,
        string BankAccount,
        decimal Amount,
        string Remark,
        DateTimeOffset CreateOn,
        DateTimeOffset UpdateOn);
}
