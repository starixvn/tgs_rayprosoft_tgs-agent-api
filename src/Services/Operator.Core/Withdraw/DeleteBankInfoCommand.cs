using System;

namespace TGS.Services.Operator.Withdraw
{
    public record DeleteBankInfoCommand(
        string PlayerCode,
        Guid BankInfoId);
}
