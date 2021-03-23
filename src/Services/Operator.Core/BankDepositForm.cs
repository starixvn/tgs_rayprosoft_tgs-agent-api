using System;

namespace TGS.Services.Operator
{
    public record BankDepositForm(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        string BankAccount,
        decimal Amount);
}
