using System;

namespace TGS.Services.Payment
{
    public record BankInfo(
        Guid Id,
        string PlayerCode,
        Guid BrandId,
        Guid PaymentChannelId,
        string BankType,
        string Detail);
}
