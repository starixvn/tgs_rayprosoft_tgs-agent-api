using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Vip
{
    public class GetVipReportReportRequest : PageableRequest
    {
        public Guid BrandId { get; init; }

        public string Username { get; init; }

        public int? Level { get; init; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }
    }
}