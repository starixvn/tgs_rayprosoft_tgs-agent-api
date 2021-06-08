using System;

namespace TGS.Services.Agent
{
    public record AgentInfo(
        Guid Id,
        string UserName,
        Guid DefaultWalletId,
        byte[] PayPassword,
        byte[] PaySalt);
}
