using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Round
{
    public class GetClosedRoundsReportRequest : PageableRequest
    {
        public string ExternalRoundId { get; set; }

        public Guid? GameProviderId { get; set; }
    }
}