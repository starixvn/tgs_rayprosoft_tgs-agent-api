using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Agent
{
    public class GetAgentDividendDetailsReportRequest : PageableRequest
    {
        public Guid AgentId { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }
    }
}