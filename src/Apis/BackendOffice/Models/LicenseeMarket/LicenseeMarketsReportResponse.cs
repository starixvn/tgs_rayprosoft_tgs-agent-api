using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.LicenseeMarket
{
    public record LicenseeMarketsReportResponse
    {
        public PagedResponse<LicenseeMarketDataReportModel> Datas { get; init; }

        public LicenseeMarketTotalReportModel[] Totals { get; init; }
    }
}