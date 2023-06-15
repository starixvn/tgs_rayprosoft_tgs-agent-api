using System;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public record GetAgentTotalDetailQuery
{
    public Guid BrandId { get; init; }
    public Guid AgentId { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
}