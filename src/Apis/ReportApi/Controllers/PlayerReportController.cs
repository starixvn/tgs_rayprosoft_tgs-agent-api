using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Application.Models.PlayerRebate;

namespace TGS.Apis.ReportApi.Controllers
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
        [Description("Get player contributions")]
        public Task<PagedResponse<PlayerContributionModel>> GetPlayerContributionsAsync(
            [FromQuery] GetPlayerContributionsRequest request) =>
            _mediator.Send(request);

        [HttpPost("rebates")]
        [Description("Create player rebates")]
        public Task<MessageResponse> CreatePlayerRebatesAsync(CreatePlayerRebateRequest request) =>
            _mediator.Send(request);

        [HttpGet("accountChanges")]
        [Description("Get player account changes")]
        public Task<PagedResponse<PlayerAccountChangeModel>> GetPlayerAccountChangesAsync(
            [FromQuery] GetPlayerAccountChangesRequest request) =>
            _mediator.Send(request);

        [HttpGet("exportPlayerContributions")]
        [Description("export player contributions")]
        public async Task<IActionResult> ExportPlayerContributionsAsync(
            [FromQuery] ExportPlayerContributionsRequest request) => File(
            await _mediator.Send(request).ConfigureAwait(false),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}