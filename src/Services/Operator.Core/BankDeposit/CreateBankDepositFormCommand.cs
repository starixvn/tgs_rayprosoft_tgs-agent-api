using System;

namespace TGS.Services.Operator.BankDeposit
{
    public record CreateBankDepositFormCommand(
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        string BankAccount,
        decimal Amount);
}
