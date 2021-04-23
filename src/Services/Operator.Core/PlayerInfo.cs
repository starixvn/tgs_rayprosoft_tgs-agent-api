using System;

namespace TGS.Services.Operator
{
    public record PlayerInfo(
        string PlayerCode,
        decimal DepositedAmount,
        bool IdentityAuthentication,
        bool PhoneAuthentication,
        Guid? VipLevel);
}
