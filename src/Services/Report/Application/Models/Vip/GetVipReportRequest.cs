using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Vip
{
    public class GetVipReportRequest : PageableRequest, IRequest<PagedResponse<VipModel>>
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public int? Level { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}