using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.LicenseeMarket
{
    public class GetLicenseeMarketsRequest : PageableRequest, IRequest<PagedResponse<LicenseeMarketModel>>
    {
        public Guid LicenseeId { get; set; }
        public string CurrencyCode { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}
