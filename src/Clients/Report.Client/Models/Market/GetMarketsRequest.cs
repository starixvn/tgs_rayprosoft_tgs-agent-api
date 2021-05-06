using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Market
{
    public class GetMarketsRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}