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
            var response = await _mediator.Send(request);

            var playerBonus =
                response.PromotionBonus.Select(x =>
                {
                    var players = x.Players.Select(p => new PlayerBonus(p.PlayerCode, p.BonusAmount));
                    return new PromotionPlayerBonus(players, x.PromotionId, x.Id, x.PromotionType);
                });

            var createPlayerBonusRequest =
                new SentPlayerPromotionBonusRequest(playerBonus, TransactionType.Promotion);

            await _mediator.Send(createPlayerBonusRequest);

            return new() {Valid = true};
        }


        [HttpPost("settleDataWeekly")]
        [Description("settleDataWeekly")]
        public async Task<MessageResponse> GetAgentTeamInformationAsync(CreatePromotionBonusWeeklyRequest request)
        {
            var response = await _mediator.Send(request);

            var playerBonus =
                response.PromotionBonus.Select(x =>
                {
                    var players = x.Players.Select(p => new PlayerBonus(p.PlayerCode, p.BonusAmount));
                    return new PromotionPlayerBonus(players, x.PromotionId, x.Id, x.PromotionType);
                });

            var createPlayerBonusRequest =
                new SentPlayerPromotionBonusRequest(playerBonus, TransactionType.Promotion);

            return await _mediator.Send(createPlayerBonusRequest);
        }

        [HttpPost("settleDataMonthly")]
        [Description("settleDataMonthly")]
        public async Task<MessageResponse>
            GetAgentDividendsAsync(CreatePromotionBonusMonthlyRequest request)
        {
            var response = await _mediator.Send(request);

            var playerBonus =
                response.PromotionBonus.Select(x =>
                {
                    var players = x.Players.Select(p => new PlayerBonus(p.PlayerCode, p.BonusAmount));
                    return new PromotionPlayerBonus(players, x.PromotionId, x.Id, x.PromotionType);
                });

            var sentPlayerPromotionBonusRequest =
                new SentPlayerPromotionBonusRequest(playerBonus, TransactionType.Promotion);

            return await _mediator.Send(sentPlayerPromotionBonusRequest);
        }
    }
}