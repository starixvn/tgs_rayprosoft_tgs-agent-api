using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Round
{
    public class GetClosedRoundsRequest : PageableRequest
    {
        public string ExternalRoundId { get; set; }

        public Guid? GameProviderId { get; set; }
    }
}
