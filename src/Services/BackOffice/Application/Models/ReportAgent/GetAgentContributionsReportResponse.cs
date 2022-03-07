namespace TGS.Services.BackOffice.Application.Models.ReportAgent
{
    public record GetAgentContributionsReportResponse
    {
        public int TotalCount { get; init; }

        public AgentContributionReportModel[] Items { get; init; }

        public AgentContributionTotalModel Total { get; init; }
    }
}