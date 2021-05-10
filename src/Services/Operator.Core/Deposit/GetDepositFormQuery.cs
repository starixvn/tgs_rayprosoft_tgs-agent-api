using System;

namespace TGS.Services.Operator.Deposit
{
    public record GetDepositFormQuery(string PlayerCode, Guid FormId);
}
