using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models.ReportAgentWinLoss;

public class GetRoundsReportRequest : PageableRequest
{
    public string PlayerCode { get; init; }

    public DateTimeOffset ClosedFrom { get; init; }

    public DateTimeOffset ClosedTo { get; init; }
}