using System;

namespace TGS.Services.Operator.ApplicationServices.Withdraw
{
    public record CreateBankInfoCommand(
        Guid BrandId,
        string PlayerCode,
        Guid PaymentChannelId,
        string BankType,
        string Detail);
}
