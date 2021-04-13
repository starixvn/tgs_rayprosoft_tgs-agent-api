using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class GetAgentDividendDetailsRequest : PageableRequest, IRequest<PagedResponse<AgentDividendDetailModel>>
    {
        public Guid AgentId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}