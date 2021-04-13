using System;

namespace TGS.Services.Operator
{
    public record BrandPayment(
        Guid Id,
        Guid PaymentMethodChannelId,
        Guid PaymentId,
        Guid MethodId,
        Guid ChannelId,
        string ChannelName,
        string ExtendedData);
}
