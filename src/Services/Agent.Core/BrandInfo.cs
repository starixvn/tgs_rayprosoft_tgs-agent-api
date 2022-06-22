using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent;

public record BrandInfo(
    Guid BrandId,
    string Code,
    bool AgentSmsEnabled,
    bool SmsRegisterFlowEnabled,
    AgentDividendType AgentDividendType,
    string? DomainUrl);
