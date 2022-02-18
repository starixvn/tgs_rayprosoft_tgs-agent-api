using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Round
{
    public class GetOpenRoundsReportRequest : PageableRequest
    {
        public string ExternalRoundId { get; init; }

        public Guid? GameProviderId { get; init; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }
    }
}