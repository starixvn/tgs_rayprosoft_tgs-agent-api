using System;

namespace TGS.Services.Agent.Services.BankInfos
{
    public record DeleteBankInfoCommand(
        Guid AgentId,
        Guid BankInfoId);
}
