using System;
using Tgs.Report.Client.Models;

namespace TGS.Services.BackOffice.Application.Models.ReportMarketing
{
    public record GetMarketingFirstDepositDetailReportRequest : PageableRequest
    {
        public Guid BrandId { get; init; }
        public string AgentUsername { get; init; } = default!;
        public DateTimeOffset Date { get; init; }
    }
}