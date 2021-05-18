using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Agent
{
    public class
        GetAgentAccountChangeQueryHandler : Handler<GetAgentAccountChangesRequest,
            PagedResponse<AgentAccountChangeModel>>
    {
        private readonly ITgsAgentService _tgsAgentService;

        public GetAgentAccountChangeQueryHandler(ITgsAgentService tgsAgentService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _tgsAgentService = tgsAgentService;
        }

        public override Task<PagedResponse<AgentAccountChangeModel>> Handle(
            GetAgentAccountChangesRequest request,
            CancellationToken cancellationToken) =>
            _tgsAgentService.GetAgentTransactionQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.AgentId == request.AgentId)
                .Where(request.TransactionType, x => x.Type == request.TransactionType)
                .Where(x => x.RecordedOn >= request.From)
                .Where(x => x.RecordedOn < request.To)
                .Select(x => new AgentAccountChangeModel
                {
                    TransactionId = x.Id,
                    Amount = x.Amount,
                    Balance = x.Balance,
                    TransactionType = x.Type,
                    RecordedOn = x.RecordedOn
                }).CreatePagedResponseAsync(request);
    }
}