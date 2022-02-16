using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Vip
{
    public class GetVipRebateReportReportRequest : PageableRequest
    {
        public Guid BrandId { get; init; }

        public string Username { get; set; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }
    }
}