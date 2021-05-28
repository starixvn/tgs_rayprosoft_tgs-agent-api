using System;

namespace TGS.Services.Operator.Services.Withdraw
{
    public record CreateBankInfoCommand(
        Guid BrandId,
        string PlayerCode,
        Guid PaymentChannelId,
        string BankType,
        string Detail);
}
