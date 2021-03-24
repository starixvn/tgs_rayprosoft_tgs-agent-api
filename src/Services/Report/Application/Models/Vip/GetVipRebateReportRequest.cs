using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Vip
{
    public class GetVipRebateReportRequest : PageableRequest, IRequest<PagedResponse<VipRebateModel>>
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}