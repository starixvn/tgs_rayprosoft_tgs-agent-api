using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Player;

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
    }
}