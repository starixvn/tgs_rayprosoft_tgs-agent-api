using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Player
{
    public class GetPlayerAccountChangesReportRequest : PageableRequest
    {
        public Guid BrandId { get; init; }

        public string PlayerCode { get; init; } = default!;

        public AccountChangeType? AccountChangeType { get; init; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }
    }
}