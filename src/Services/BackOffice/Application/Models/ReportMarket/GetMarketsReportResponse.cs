namespace TGS.Services.BackOffice.Application.Models.ReportMarket
{
    public class GetMarketsReportResponse
    {
        public int TotalCount { get; init; }

        public MarketReportModel[] Items { get; init; }

        public MarketReportTotalModel Total { get; init; }
    }
}