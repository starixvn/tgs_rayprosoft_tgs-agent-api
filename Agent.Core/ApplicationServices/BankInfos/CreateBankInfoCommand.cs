using System;

namespace TGS.Services.Agent.ApplicationServices.BankInfos;

public record CreateBankInfoCommand(
    Guid BrandId,
    Guid AgentId,
    Guid PaymentChannelId,
    string BankType,
    string Detail);
