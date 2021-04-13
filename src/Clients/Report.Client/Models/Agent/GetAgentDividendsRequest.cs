using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace Report.Client.Models.Agent
{
    public class GetAgentDividendsRequest : PageableRequest
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public AgentType? Type { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}