using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Agent
{
    public class GetAgentPlayerWinLossReportRequest : PageableRequest
    {
        public Guid AgentId { get; init; }

        public DateTimeOffset From { get; init; }

        public DateTimeOffset To { get; init; }
    }
}