using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Round
{
    public class GetOpenRoundsRequest : PageableRequest
    {
        public string ExternalRoundId { get; set; }

        public Guid? GameProviderId { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }
    }
}