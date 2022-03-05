#nullable enable
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using TGS.Services.BackOffice.Application.Models.ReportRound;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public RoundReportController(IReportClient client) => _client = client;

        [HttpGet("closedRounds")]
        [Description("Get closed rounds")]
        public async Task<PagedResponse<ClosedRoundReportModel>> GetClosedRoundsAsync(
            [FromServices] IGetService<GetClosedRoundsReportRequest, PagedResponse<ClosedRoundReportModel>>
                closedRoundsService,
            [FromQuery] GetClosedRoundsReportRequest request)
            => await closedRoundsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("openRounds")]
        [Description("Get open rounds")]
        public async Task<PagedResponse<OpenRoundReportModel>> GetOpenRoundsAsync(
            [FromServices] IGetService<GetOpenRoundsReportRequest, PagedResponse<OpenRoundReportModel>>
                openRoundsService,
            [FromQuery] GetOpenRoundsReportRequest request)
            => await openRoundsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet("{roundId}")]
        [Description("Get a round")]
        public async Task<JsonDocument?> GetRoundHistoryAsync(Guid roundId,
            [FromServices] IGetService<GetRoundHistoryReportRequest, JsonDocument?>
                roundHistoryService,
            [FromQuery] GetRoundHistoryReportRequest request)
            => await roundHistoryService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpGet]
        [Description("Get rounds")]
        public async ValueTask<PagedResponse<RoundView>> GetRoundsAsync(
            [FromServices] IGetService<GetRoundsReportRequest, PagedResponse<RoundView>>
                roundsService,
            [FromQuery] GetRoundsReportRequest request)
            => await roundsService
                .GetAsync(request)
                .ConfigureAwait(false);
    }
}