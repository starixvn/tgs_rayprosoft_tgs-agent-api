using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Report.Client;
using Report.Client.Models.Agent;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public AgentReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet("contribution")]
        [Description("Get agent contributions")]
        public Task<PagedResponse<AgentContributionModel>> GetRoundsAsync(
            [FromQuery] GetAgentContributionsRequest request) =>
            _client.GetAgentContributionsAsync(request);

        [HttpGet("{id}/teamInfo")]
        [Description("Get agent team Information")]
        public Task<PagedResponse<TeamInfoModel>> GetAgentTeamInformationAsync(Guid id) =>
            _client.GetAgentTeamInfoAsync(new(id));

        [HttpGet("bonus")]
        [Description("Get agent bonus report")]
        public Task<PagedResponse<AgentDividendModel>>
            GetAgentBonusAsync([FromQuery] GetAgentDividendRequest request) =>
            _client.GetAgentBonusAsync(request);

        [HttpGet("{id}/winLoss")]
        [Description("Get agent player Win/Loss report")]
        public Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(Guid id,
            [FromQuery] GetAgentPlayerWinLossRequest request)
        {
            request.AgentId = id;
            return _client.GetAgentPlayerWinLossAsync(request);
        }
    }
}