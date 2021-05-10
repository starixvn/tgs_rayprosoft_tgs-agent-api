using System;
using TGS.Core.Models.Enums;

namespace TGS.Clients.Report.Client.Models.Agent
{
    public class GetAgentContributionsRequest
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public AgentType? Type { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public bool IncludeAgents { get; set; }
    }
}