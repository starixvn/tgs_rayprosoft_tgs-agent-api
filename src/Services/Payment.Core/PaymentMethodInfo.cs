using System;

namespace TGS.Services.Payment
{
    public record PaymentMethodInfo(Guid Id, string Name, string BankType);
}
