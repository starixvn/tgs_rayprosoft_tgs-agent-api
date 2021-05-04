using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Vip
{
    public class GetVipReportRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public int? Level { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}