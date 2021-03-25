using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Player
{
    public class GetAgentTeamInfoRequest : PageableRequest, IRequest<PagedResponse<PlayerModel>>
    {
        public GetAgentTeamInfoRequest(Guid id)
        {
            AgentId = id;
        }

        public Guid AgentId { get; set; }
    }
}