using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class GetAgentTeamInfoRequest : PageableRequest, IRequest<PagedResponse<TeamInfoModel>>
    {
        public Guid AgentId { get; set; }
    }
}