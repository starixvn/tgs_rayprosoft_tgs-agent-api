using System;

namespace TGS.Services.Operator.Services.Deposit
{
    public record GetDepositFormQuery(string PlayerCode, Guid FormId);
}
