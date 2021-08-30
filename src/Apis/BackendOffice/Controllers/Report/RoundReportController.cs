using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.Game;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public RoundReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet("{roundId}")]
        [Description("Get a round")]
        public Task<GetRoundHistoryResponse> GetRoundAsync(Guid roundId, [FromQuery] GetRoundHistoryRequest request)
        {
            request.RoundId = roundId;
            return _client.GetRoundHistoriesAsync(request);
        }

        [HttpGet]
        [Description("Get rounds")]
        public async ValueTask<PagedResponse<RoundModel>> GetRoundsAsync(
            [FromServices] IGetService<GetGameRoundListQuery, PagedResponse<RoundModel>> queryService,
            [FromServices] IGetService<GetAllGamesByBrandIdQuery, GameData> gameQueryService,
            [FromQuery] GetRoundsRequest request)
        {
            var query = new GetGameRoundListQuery(
                request.BrandId,
                request.Username,
                request.PlayerCode,
                request.RoundId,
                request.ExternalRoundId,
                request.GameProviderId,
                request.Status,
                request.GameType,
                request.From,
                request.To,
                request);

            var records = await queryService
                .GetAsync(query)
                .ConfigureAwait(false);

            var allGames = gameQueryService.GetAsync(new GetAllGamesByBrandIdQuery(request.BrandId));

            return null;
        }
    }
}
