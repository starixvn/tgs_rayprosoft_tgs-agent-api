using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.LicenseeMarket
{
    public class GetLicenseeMarketsRequest : PageableRequest, IRequest<LicenseeMarketsResponse>
    {
        public Guid LicenseeId { get; set; }
        public string[] CurrencyCodes { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}
