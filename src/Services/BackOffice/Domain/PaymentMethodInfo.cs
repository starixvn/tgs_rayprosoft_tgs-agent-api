using System;

namespace TGS.Services.BackOffice.Domain
{
    public record PaymentMethodInfo(
        Guid Id,
        string Code);
}
