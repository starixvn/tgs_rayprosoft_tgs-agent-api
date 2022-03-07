namespace TGS.Services.BackOffice.Application.Models.ReportAgent
{
    public class GetAgentDividendsReportResponse
    {
        public int TotalCount { get; init; }

        public AgentDividendReportModel[] Items { get; init; }

        public AgentDividendReportTotalModel Total { get; init; }
    }
}