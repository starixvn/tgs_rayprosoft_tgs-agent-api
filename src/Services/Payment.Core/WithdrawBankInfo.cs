using System;

namespace TGS.Services.Payment
{
    public record WithdrawBankInfo<TDetail>(
        Guid Id,
        Guid PaymentChannelId,
        string BankType,
        TDetail? Detail);
}
