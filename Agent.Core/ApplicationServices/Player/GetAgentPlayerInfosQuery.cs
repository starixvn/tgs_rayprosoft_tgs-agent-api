using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record GetAgentPlayerInfosQuery(
    Guid AgentId,
    string? Username,
    DateTimeOffset? BeginRegisteredOn,
    DateTimeOffset? EndRegisteredOn,
    string? RegisterHost,
    PlayerStatus? Status,
    Guid? VipId,
    PagingInfo Paging);
