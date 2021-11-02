using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.PromotionBonus;
using TGS.Services.Promotion.Services;
using Tgs.ServiceUtility;

namespace TGS.Apis.PromotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionBonusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionBonusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("settleDataDaily")]
        [Description("settleDataDaily")]
        public Task<MessageResponse> SettleDataDailyAsync(
            CreatePromotionBonusDailyRequest request) =>
            _mediator.Send(request);


        [HttpPost("settleDataWeekly")]
        [Description("settleDataWeekly")]
        public Task<MessageResponse> SettleDataWeeklyAsync(CreatePromotionBonusWeeklyRequest request) =>
            _mediator.Send(request);

        [HttpPost("settleDataMonthly")]
        [Description("settleDataMonthly")]
        public Task<MessageResponse>
            SettleDataMonthlyAsync(CreatePromotionBonusMonthlyRequest request) =>
            _mediator.Send(request);

        [HttpGet("bonusAmount")]
        [Description("Get bonus amount by ids")]
        public ValueTask<PromotionBonusOfDepositModel[]> GetBonusAmountByIdsAsync(
            [FromServices] IQueryService<GetPromotionBonusOfDepositQuery, PromotionBonusOfDepositModel> queryService,
            [FromQuery] GetPromotionBonusOfDepositQuery query)
            => queryService.QueryAsync(query).ToArrayAsync();
    }
}