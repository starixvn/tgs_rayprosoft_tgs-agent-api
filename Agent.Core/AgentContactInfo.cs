using System;

namespace TGS.Services.Agent;

public record AgentContactInfo(
    Guid Id,
    string Username,
    string? RealName,
    string? Phone,
    string? Email,
    string? QQ,
    string? Wechat
);