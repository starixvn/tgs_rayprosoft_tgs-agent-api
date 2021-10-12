using System;
using System.Collections.Generic;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent
{
    public record AgentInfo(
        Guid Id,
        string Username,
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
        byte[] Password,
        byte[] Salt,
        decimal AccumulatedProfitSharing,
        decimal DividendPercentage,
        string? ParentUserName,
        DateTimeOffset RegisteredOn,
        decimal Balance,
        AgentStatus Status,
        AgentType Type,
        AgentLevelType AgentLevelType,
        IEnumerable<AgentDividendCondition> AgentDividendConditions);
}
