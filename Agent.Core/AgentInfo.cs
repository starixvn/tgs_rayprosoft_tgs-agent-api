using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent;

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
    byte[] PayPassword,
    byte[] PaySalt,
    byte[] Password,
    byte[] Salt,
    Guid? ParentAgentId,
    DateTimeOffset RegisteredOn,
    AgentStatus Status,
    AgentType Type,
    AgentLevelType AgentLevelType,
    decimal DividendPercentage,
    int PlayerNumber,
    decimal AccumulatedProfitSharing);
