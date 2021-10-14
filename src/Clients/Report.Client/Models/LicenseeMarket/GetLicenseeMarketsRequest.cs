using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.LicenseeMarket
{
    public class GetLicenseeMarketsRequest : PageableRequest
    {
        public Guid LicenseeId { get; set; }
        public string CurrencyCode { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}
