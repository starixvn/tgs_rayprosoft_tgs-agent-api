using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class GetAgentContributionsRequest : PageableRequest, IRequest<PagedResponse<AgentContributionModel>>
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public AgentType Type { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public bool IncludeAgents { get; set; }
    }
}