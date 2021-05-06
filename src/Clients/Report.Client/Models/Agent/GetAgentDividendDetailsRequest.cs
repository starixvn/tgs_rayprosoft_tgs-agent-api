using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Agent
{
    public class GetAgentDividendDetailsRequest : PageableRequest
    {
        public Guid AgentId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}