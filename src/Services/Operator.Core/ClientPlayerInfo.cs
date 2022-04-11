using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record ClientPlayerInfo(
        string PlayerCode,
        Guid BrandId,
        decimal DepositedAmount,
        bool IdentityAuthentication,
        bool PhoneAuthentication,
        string? Phone,
        string? AreaCode,
        Guid? VipLevelId,
        PlayerStatus Status,
        string UserName,
        string? RealName,
        string? Email,
        int VipLevel,
        string? QQ,
        string? Wechat,
        bool IsSetPayPassword,
        string? FacebookMessenger,
        string? Line,
        string? Viber,
        string? Telegram,
        string? WhatsApp);
}
