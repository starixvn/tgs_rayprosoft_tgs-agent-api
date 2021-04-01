using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.PluggableServices.Models;
using TGS.Services.Report.Application.Models.Round;

namespace ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoundReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get rounds")]
        public Task<PagedResponse<RoundModel>> GetRoundsAsync([FromQuery] GetRoundsRequest request) =>
            _mediator.Send(request);

        [HttpGet("{roundId}")]
        [Description("Get a round")]
        public Task<GetRoundHistoryResponse> GetRoundAsync(Guid roundId, [FromQuery] GetRoundHistoryRequest request)
        {
            request.RoundId = roundId;
            return _mediator.Send(request);
        }
    }
}