using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Bonus.Models;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Application.Models.PlayerRebate;

namespace ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayerReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("contribution")]
        [Description("Get agent contributions")]
        public Task<PagedResponse<PlayerContributionModel>> GetRoundsAsync(
            [FromQuery] GetPlayerContributionsRequest request) =>
            _mediator.Send(request);

        [HttpPost("rebates")]
        [Description("Create player rebates")]
        public async Task<MessageResponse> GetRoundsAsync(CreatePlayerRebateRequest request)
        {
            var response = await _mediator.Send(request);

            if (response.PlayerRebates.IsNullOrEmpty())
            {
                return response;
            }

            var playerBonus =
                response.PlayerRebates.Select(x => new PlayerRebates(x.PlayerCode, x.RebateAmount, x.ReportDate));

            var sentPlayerRebatesRequest =
                new SentPlayerRebatesRequest(playerBonus, TransactionType.Rebate);

            return await _mediator.Send(sentPlayerRebatesRequest);
        }
    }
}