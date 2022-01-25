using System;
using TGS.Core.Models.Enums;

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
        Guid? VipLevel,
        PlayerStatus Status,
        PlayerType Type);
}
