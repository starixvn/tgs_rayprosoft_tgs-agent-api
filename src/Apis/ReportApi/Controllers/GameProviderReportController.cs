using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.GameProvider;

namespace TGS.Apis.ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameProviderReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameProviderReportController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [Description("Get game provider round aggregate")]
        public Task<PagedResponse<GameProviderRoundsModel>> GetGameProviderRoundsAsync(
            [FromQuery] GetGameProviderRoundsRequest request) =>
            _mediator.Send(request);
    }
}