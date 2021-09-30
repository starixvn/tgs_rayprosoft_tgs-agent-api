using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.LicenseeMarket
{
    public class LicenseeMarketsResponse
    {
        public PagedResponse<LicenseeMarketDataModel> Datas { get; set; }
        public LicenseeMarketTotalModel[] Totals { get; set; }

    }
}
