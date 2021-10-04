using System;

namespace TGS.Services.Operator
{
    public record PlayerInfo(
        string PlayerCode,
        Guid BrandId,
        decimal DepositedAmount,
        bool IdentityAuthentication,
        bool PhoneAuthentication,
        string? Phone,
        string? AreaCode,
        Guid? VipLevel);
}
