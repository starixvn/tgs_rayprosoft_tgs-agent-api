using System;

namespace TGS.Services.Agent.ApplicationServices.BankInfos
{
    public record DeleteBankInfoCommand(
        Guid AgentId,
        Guid BankInfoId);
}
