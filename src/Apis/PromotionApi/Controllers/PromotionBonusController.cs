using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.PromotionBonus;

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
        public Task<MessageResponse> GetRoundsAsync(
            CreatePromotionBonusDailyRequest request) =>
            _mediator.Send(request);


        [HttpPost("settleDataWeekly")]
        [Description("settleDataWeekly")]
        public Task<MessageResponse> GetAgentTeamInformationAsync(CreatePromotionBonusWeeklyRequest request) =>
            _mediator.Send(request);

        [HttpPost("settleDataMonthly")]
        [Description("settleDataMonthly")]
        public Task<MessageResponse>
            GetAgentDividendsAsync(CreatePromotionBonusMonthlyRequest request) =>
            _mediator.Send(request);
    }
}