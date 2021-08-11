using System;

namespace TGS.Services.Agent
{
    public record AgentInfo(
        Guid Id,
        string UserName,
        string? RealName,
        string? Phone,
        Guid DefaultWalletId,
        string? Email,
        string? QQ,
        string? Wechat,
        string InvitationCode,
        string? BrandUrl,
        int AgentLevel,
        byte[] PayPassword,
        byte[] PaySalt,
        decimal AccumulatedProfitSharing);
}