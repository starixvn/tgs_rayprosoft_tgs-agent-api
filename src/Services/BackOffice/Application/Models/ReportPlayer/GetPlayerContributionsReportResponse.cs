namespace TGS.Services.BackOffice.Application.Models.ReportPlayer
{
    public class GetPlayerContributionsReportResponse
    {
        public int TotalCount { get; init; }

        public PlayerContributionReportModel[] Items { get; init; }

        public PlayerContributionReportTotalModel Total { get; init; }
    }
}