using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Player;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public PlayerReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet("contribution")]
        [Description("Get agent contributions")]
        public Task<PagedResponse<PlayerContributionModel>> GetRoundsAsync(
            [FromQuery] GetPlayerContributionsRequest request) =>
            _client.GetPlayerContributionsAsync(request);

        [HttpGet("exportPlayerContributions")]
        public async Task<IActionResult> ExportPlayerContributionsAsync(
            [FromQuery] ExportPlayerContributionsRequest request)
            => File(
                await _client.ExportPlayerContributionsAsync(request).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PlayerContributions.xlsx");
    }
}