using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Player
{
    public class GetPlayersAccountChangesReportRequest : PageableRequest
    {
        public Guid BrandId { get; init; }

        public string Username { get; init; } = default!;

        public TransactionType? TransactionType { get; init; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }
    }
}