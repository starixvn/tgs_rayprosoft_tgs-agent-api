using System;

namespace TGS.Services.Operator.Services.Withdraw
{
    public record DeleteBankInfoCommand(
        string PlayerCode,
        Guid BankInfoId);
}
