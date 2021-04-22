using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Bonus.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Promotion;

namespace ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("settleDataDaily")]
        [Description("settleDataDaily")]
        public async Task<MessageResponse> GetRoundsAsync(
            CreatePromotionBonusDailyRequest request)
        {
            var promotionBonusPlayers = await _mediator.Send(request);
            var playerBonus =
                promotionBonusPlayers.Select(x => new PlayerBonus(x.PlayerCode, x.BonusAmount, x.PromotionType))
                    .ToArray();
            var createPlayerBonusRequest = new CreatePlayerBonusRequest(playerBonus, TransactionType.Promotion, true);

            await _mediator.Send(createPlayerBonusRequest);

            return new() {Valid = true};
        }


        [HttpPost("settleDataWeekly")]
        [Description("settleDataWeekly")]
        public async Task<MessageResponse> GetAgentTeamInformationAsync(CreatePromotionBonusWeeklyRequest request)
        {
            var promotionBonusPlayers = await _mediator.Send(request);
            var playerBonus =
                promotionBonusPlayers.Select(x => new PlayerBonus(x.PlayerCode, x.BonusAmount, x.PromotionType))
                    .ToArray();
            var createPlayerBonusRequest = new CreatePlayerBonusRequest(playerBonus, TransactionType.Promotion, true);

            await _mediator.Send(createPlayerBonusRequest);

            return new() {Valid = true};
        }

        [HttpPost("settleDataMonthly")]
        [Description("settleDataMonthly")]
        public async Task<MessageResponse>
            GetAgentDividendsAsync(CreatePromotionBonusMonthlyRequest request)
        {
            var promotionBonusPlayers = await _mediator.Send(request);
            var playerBonus =
                promotionBonusPlayers.Select(x => new PlayerBonus(x.PlayerCode, x.BonusAmount, x.PromotionType))
                    .ToArray();
            var createPlayerBonusRequest = new CreatePlayerBonusRequest(playerBonus, TransactionType.Promotion, true);

            await _mediator.Send(createPlayerBonusRequest);

            return new() {Valid = true};
        }
    }
}