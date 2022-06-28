#nullable enable

using System;
using Tgs.Report.Client.Models;

namespace TGS.Services.BackOffice.Application.Models.ReportMarketing;

public record GetMarketingListReportRequest : PageableRequest
{
    public Guid BrandId { get; init; }
    public string? AgentUsername { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
}
