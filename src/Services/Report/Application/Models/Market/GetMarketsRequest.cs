using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Market
{
    public class GetMarketsRequest : PageableRequest, IRequest<PagedResponse<MarketModel>>
    {
        public Guid BrandId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}