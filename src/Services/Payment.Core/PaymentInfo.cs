using System;

namespace TGS.Services.Payment
{
    public record PaymentInfo(
        Guid Id,
        string Code);
}
