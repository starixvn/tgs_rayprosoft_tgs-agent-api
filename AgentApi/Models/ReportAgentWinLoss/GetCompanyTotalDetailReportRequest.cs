using System;

namespace TGS.Apis.AgentApi.Models.ReportAgentWinLoss;

public record GetCompanyTotalDetailReportRequest
{
    public Guid BrandId { get; init; }
    public Guid AgentId { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
}