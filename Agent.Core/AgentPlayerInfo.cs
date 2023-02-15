using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent;

public record AgentPlayerInfo(
    string PlayerCode,
    string Username,
    DateTimeOffset? RegisteredTime,
    string? AreaCode,
    string? Phone,
    string? RegisterHost,
    PlayerStatus Status,
    int? VipLevel);
