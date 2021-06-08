using System;

namespace TGS.Services.Agent.Services.BankInfos
{
    public record CreateBankInfoCommand(
        Guid BrandId,
        Guid AgentId,
        Guid PaymentChannelId,
        string BankType,
        string Detail);
}
