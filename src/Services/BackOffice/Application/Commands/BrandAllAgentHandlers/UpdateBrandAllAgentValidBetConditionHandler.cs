using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandAllAgent;
using TGS.Services.BackOffice.Infrastructure.DataService;
using Brand = TGS.Services.BackOffice.Application.Resources.Brand;

namespace TGS.Services.BackOffice.Application.Commands.BrandAllAgentGameTypeConditionHandlers
{
    public class
        UpdateBrandAllAgentValidBetConditionHandler : Handler<UpdateBrandAllAgentValidBetConditionRequest,
            MessageResponse>
    {
        private readonly IBrandService _brandService;
        private readonly IBrandAllAgentGameTypeConditionService _service;

        public UpdateBrandAllAgentValidBetConditionHandler(IBrandService brandService,
            IBrandAllAgentGameTypeConditionService service,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _brandService = brandService;
        }

        public override async Task<MessageResponse> Handle(UpdateBrandAllAgentValidBetConditionRequest request,
            CancellationToken cancellationToken)
        {
            var brand = await _brandService.GetBrandAsync(x => x.Id == request.BrandId);

            if (brand == null)
            {
                return CreateInvalidResponse<MessageResponse>(Brand.NotFound);
            }

            var conditions =
                await _service.GetBrandAllAgentGameTypeConditionQueries().Where(x
                    => x.BrandId == request.BrandId && x.GameType == request.GameType &&
                    x.RebateType == AllAgentRebateType.ValidBet).ToArrayAsync(cancellationToken);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _service.DeleteBrandAllAgentGameTypeConditionsAsync(conditions);
            await _service.CreateBrandAllAgentGameTypeConditionsAsync(request.Conditions.Select(x
                => new BrandAllAgentGameTypeCondition
                {
                    Rank = x.Rank,
                    BrandId = request.BrandId,
                    NetProfit = x.NetProfit,
                    ActiveNumber = x.ActiveNumber,
                    DividendPercentage = x.DividendPercentage,
                    GameType = request.GameType,
                    RebateType = AllAgentRebateType.ValidBet
                }).ToArray());

            scope.Complete();

            return CreateValidResponse<MessageResponse>();
        }
    }
}