using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Agent;
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

        [HttpGet("contributions")]
        [Description("Get agent contributions")]
        public Task<PagedResponse<AgentContributionModel>> GetRoundsAsync(
            [FromQuery] GetAgentContributionsRequest request) =>
            _client.GetAgentContributionsAsync(request);

        [HttpGet("{id}/teamInfo")]
        [Description("Get agent team Information")]
        public Task<PagedResponse<TeamInfoModel>> GetAgentTeamInformationAsync(Guid id,
            [FromQuery] GetAgentTeamInfoRequest request)
        {
            request.AgentId = id;
            return _client.GetAgentTeamInfoAsync(request);
        }

        [HttpGet("dividends")]
        [Description("Get agent dividends report")]
        public Task<PagedResponse<AgentDividendModel>>
            GetAgentDividendsAsync([FromQuery] GetAgentDividendsRequest request) =>
            _client.GetAgentDividendsAsync(request);

        [HttpGet("dividendDetails")]
        [Description("Get agent dividend details report")]
        public Task<PagedResponse<AgentDividendDetailModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetAgentDividendDetailsRequest request) =>
            _client.GetAgentDividendDetailsAsync(request);

        [HttpGet("{id}/winLoss")]
        [Description("Get agent player Win/Loss report")]
        public Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(Guid id,
            [FromQuery] GetAgentPlayerWinLossRequest request)
        {
            request.AgentId = id;
            return _client.GetAgentPlayerWinLossAsync(request);
        }

        [HttpGet("exportAgentContributions")]
        public async Task<IActionResult> GetAgentContributionsFileAsync(
            [FromQuery] ExportAgentContributionsRequest request)
        {
            Request.Headers.TryGetValue("Accept-Language", out var acceptLanguage);
            request.AcceptLanguage = acceptLanguage.FirstOrDefault() ?? CultureInfo.CurrentCulture.Name;

            return File(
                await _client.ExportAgentContributionsAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AgentContributions.xlsx");
        }

        [HttpGet("exportAgentDividends")]
        public async Task<IActionResult> GetAgentDividendsFileAsync(
            [FromQuery] ExportAgentDividendsRequest request)
        {
            Request.Headers.TryGetValue("Accept-Language", out var acceptLanguage);
            request.AcceptLanguage = acceptLanguage.FirstOrDefault() ?? CultureInfo.CurrentCulture.Name;

            return File(
                await _client.ExportAgentDividendsAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AgentDividends.xlsx");
        }
    }
}