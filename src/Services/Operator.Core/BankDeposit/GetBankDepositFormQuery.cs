using System;

namespace TGS.Services.Operator.BankDeposit
{
    public record GetBankDepositFormQuery(string PlayerCode, Guid FormId);
}
