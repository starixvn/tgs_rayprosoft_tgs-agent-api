using System;

namespace TGS.Services.Agent
{
    public record AgentInfo(
        Guid Id,
        string UserName,
        Guid DefaultWalletId,
        string Email,
        byte[] PayPassword,
        byte[] PaySalt);
}
