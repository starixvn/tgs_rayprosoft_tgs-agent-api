using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Agent
{
    public class GetAgentAccountChangesRequest : PageableRequest, IRequest<PagedResponse<AgentAccountChangeModel>>
    {
        public Guid BrandId { get; set; }
        public Guid AgentId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}