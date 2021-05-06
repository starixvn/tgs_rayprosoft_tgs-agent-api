using System;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client.Models.Agent
{
    public class GetAgentTeamInfoRequest : PageableRequest
    {
        public GetAgentTeamInfoRequest(Guid id)
        {
            AgentId = id;
        }

        public Guid AgentId { get; set; }
    }
}