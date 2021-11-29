using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandAgentDividend;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandAgentDividendHandlers
{
    public class UpdateBrandAgentDividendHandler : Handler<UpdateBrandAgentDividendConditionRequest, MessageResponse>
    {
        private readonly IBrandAgentDividendConditionService _service;

        public UpdateBrandAgentDividendHandler(IBrandAgentDividendConditionService service,
            IHttpContextAccessor accessor) : base(accessor)
            => _service = service;

        public override async Task<MessageResponse> Handle(UpdateBrandAgentDividendConditionRequest request,
            CancellationToken cancellationToken)
        {
            var brandAgentDividendConditions = request.BrandAgentDividendConditions.Select(x
                => new BrandAgentDividendCondition
                {
                    BrandId = request.BrandId,
                    Rank = x.Rank,
                    ActiveNumber = x.ActiveNumber,
                    NetProfit = x.NetProfit,
                    DividendPercentage = x.DividendPercentage
                }).ToArray();

            await _service.UpdateBrandAgentDividendConditionsAsync(request.BrandId, brandAgentDividendConditions);

            return CreateValidResponse<MessageResponse>();
        }
    }
}