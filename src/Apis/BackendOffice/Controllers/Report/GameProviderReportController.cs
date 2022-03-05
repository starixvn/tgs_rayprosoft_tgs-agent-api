using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.GameProvider;
using TGS.Services.BackOffice.Application.Models.Aggregate;
using TGS.Services.BackOffice.Application.Models.ReportGameProviderRound;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameProviderReportController : BaseContextController
    {
        [HttpGet]
        [Description("Get game provider round aggregate")]
        public async Task<PagedResponse<GameProviderRoundsModel>> GetGameProviderRoundsAsync(
            [FromServices] IGetService<GetGameProviderRoundsReportRequest, PagedResponse<GameProviderRoundsModel>>
                gameProviderRoundsService,
            [FromQuery] GetGameProviderRoundsReportRequest request)
            => await gameProviderRoundsService
                .GetAsync(request)
                .ConfigureAwait(false);

        [HttpPost("generate")]
        [Description("Generate game provider round aggregate")]
        public async Task<MessageResponse> GenerateGameProviderAggregateAsync(
            [FromServices] IGetService<GenerateGameProviderAggregateRequest, MessageResponse>
                generateGameProviderAggregateService,
            [FromBody] GenerateGameProviderAggregateRequest request)
        {
            request.UserId = UserId;
            return await generateGameProviderAggregateService
                .GetAsync(request)
                .ConfigureAwait(false);
        }
    }
}