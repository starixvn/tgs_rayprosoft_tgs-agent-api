using System;

namespace TGS.Services.Operator.Deposit
{
    public record GetBankDepositFormQuery(string PlayerCode, Guid FormId);
}
