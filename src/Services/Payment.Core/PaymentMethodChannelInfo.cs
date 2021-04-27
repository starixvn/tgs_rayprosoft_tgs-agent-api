using System;

namespace TGS.Services.Payment
{
    public record PaymentMethodChannelInfo(
        Guid Id,
        Guid PaymentId,
        Guid MethodId,
        Guid ChannelId);
}
