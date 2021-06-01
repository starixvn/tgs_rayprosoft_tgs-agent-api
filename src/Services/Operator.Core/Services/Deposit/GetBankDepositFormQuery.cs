using System;

namespace TGS.Services.Operator.Services.Deposit
{
    public record GetBankDepositFormQuery(string PlayerCode, Guid FormId);
}
