using System;

namespace TGS.Services.Operator
{
    public record BrandPayment(
        Guid Id,
        Guid PaymentMethodChannelId,
        string ChannelName,
        string ExtendedData);
}
