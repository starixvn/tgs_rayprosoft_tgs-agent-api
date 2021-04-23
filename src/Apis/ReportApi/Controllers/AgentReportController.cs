using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Bonus.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;

namespace ReportApi.Controllers
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
        public async Task<MessageResponse> CreateAgentDividendsAsync(
            [FromQuery] CreateAgentDividendsRequest request)
        {
            var response = await _mediator.Send(request);
            var agentDividends =
                response.AgentDividends.Select(x =>
                    new AgentDividend(x.BrandId, x.AgentId, x.AgentDividendAmount, x.From, x.To));

            var sentAgentDividendsRequest =
                new SentAgentDividendsRequest(agentDividends, TransactionType.Dividend);

            return await _mediator.Send(sentAgentDividendsRequest);
        }
    }
}