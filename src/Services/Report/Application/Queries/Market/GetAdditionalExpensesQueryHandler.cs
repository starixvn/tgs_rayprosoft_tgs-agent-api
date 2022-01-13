using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Services.Report.Application.Models.Market;
using TGS.Services.Report.Domain;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Market
{
    internal class
        GetAdditionalExpensesQueryHandler : Handler<GetAdditionalExpensesRequest, GetAdditionalExpensesResponse>
    {
        private readonly IPlayerAggregateService _service;

        public GetAdditionalExpensesQueryHandler(IPlayerAggregateService service, IHttpContextAccessor accessor) :
            base(accessor)
            => _service = service;

        public override async Task<GetAdditionalExpensesResponse> Handle(
            GetAdditionalExpensesRequest request,
            CancellationToken cancellationToken)
        {
            var playerAggregates = await _service.GetPlayerAggregateQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.ReportDate == request.ReportDate)
                .Select(x => new { x.AllAgentRebateAmount }).ToArrayAsync(cancellationToken);

            return new GetAdditionalExpensesResponse
            {
                AdditionalExpenses = new[]
                {
                    new AdditionalExpensesModel
                    {
                        AdditionalExpensesType = AdditionalExpensesType.AllAgent,
                        Amount = playerAggregates.Sum(x => x.AllAgentRebateAmount)
                    }
                }
            };
        }
    }
}