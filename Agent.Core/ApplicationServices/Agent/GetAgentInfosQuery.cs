using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent.ApplicationServices.Agent;

public record GetAgentInfosQuery(
    Guid BrandId,
    Guid ParentAgentId,
    AgentStatus? Status,
    string? Username
);