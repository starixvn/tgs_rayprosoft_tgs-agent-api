using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.LicenseeMarket
{
    public class LicenseeMarketsResponse
    {
        public PagedResponse<LicenseeMarketDataModel> Datas { get; set; }
        public LicenseeMarketTotalModel[] Totals { get; set; }
    }
}
