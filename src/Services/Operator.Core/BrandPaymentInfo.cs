using System;

namespace TGS.Services.Operator
{
    public record BrandPaymentInfo(
        Guid Id,
        Guid PaymentMethodChannelId,
        Guid PaymentId,
        Guid MethodId,
        Guid ChannelId,
        string ChannelName,
        string ExtendedData,
        ValueRange<decimal> TransferLimit);
}
