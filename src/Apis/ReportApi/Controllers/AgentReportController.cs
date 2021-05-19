using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;

namespace TGS.Apis.ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("contribution")]
        [Description("Get agent contributions")]
        public Task<PagedResponse<AgentContributionModel>> GetRoundsAsync(
            [FromQuery] GetAgentContributionsRequest request) =>
            _mediator.Send(request);


        [HttpGet("{id}/teamInfo")]
        [Description("Get agent team Information")]
        public Task<PagedResponse<TeamInfoModel>> GetAgentTeamInformationAsync(Guid id) =>
            _mediator.Send(new GetAgentTeamInfoRequest(id));

        [HttpGet("dividends")]
        [Description("Get agent dividends report")]
        public Task<PagedResponse<AgentDividendModel>>
            GetAgentDividendsAsync([FromQuery] GetAgentDividendsRequest request) =>
            _mediator.Send(request);

        [HttpGet("dividendDetails")]
        [Description("Get agent dividend details report")]
        public Task<PagedResponse<AgentDividendDetailModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetAgentDividendDetailsRequest request) =>
            _mediator.Send(request);

        [HttpGet("{id}/winLoss")]
        [Description("Get agent player Win/Loss report")]
        public Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(
            [FromQuery] GetAgentPlayerWinLossRequest request) =>
            _mediator.Send(request);

        [HttpPost("dividends")]
        [Description("Create agent dividends")]
        public Task<MessageResponse> CreateAgentDividendsAsync(
            CreateAgentDividendsRequest request) =>
            _mediator.Send(request);

        [HttpGet("accountChanges")]
        [Description("Get agent account changes")]
        public Task<PagedResponse<AgentAccountChangeModel>> GetAgentAccountChangesAsync(
            [FromQuery] GetAgentAccountChangesRequest request) =>
            _mediator.Send(request);
    }
}