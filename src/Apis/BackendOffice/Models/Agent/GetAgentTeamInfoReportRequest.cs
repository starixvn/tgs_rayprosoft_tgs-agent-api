using System;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Models.Agent
{
    public class GetAgentTeamInfoReportRequest : PageableRequest
    {
        public Guid AgentId { get; init; }
    }
}