using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.LicenseeMarket
{
    public class ExportLicenseeMarketsRequest : PageableRequest
    {
        public Guid LicenseeId { get; set; }
        public string[] CurrencyCodes { get; set; }
        public string CurrencyCode { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}
