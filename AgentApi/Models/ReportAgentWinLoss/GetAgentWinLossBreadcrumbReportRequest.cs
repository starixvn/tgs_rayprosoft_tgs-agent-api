#nullable enable

using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Models.ReportAgentWinLoss;

public class GetAgentWinLossBreadcrumbReportRequest
{
    public Guid BrandId { get; set; }
    public string? Username { get; init; }
    public AccountType MemberType { get; init; }
    public Guid? RoundId { get; init; }
}
