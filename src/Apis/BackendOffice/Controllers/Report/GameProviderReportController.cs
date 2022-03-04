using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.GameProvider;
using Tgs.Report.Client;
using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.GameProvider;
using TGS.Services.BackOffice.Application.Models.Aggregate;
using MessageResponse = TGS.Core.Models.Messages.MessageResponse;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameProviderReportController : BaseContextController
    {
        private readonly IReportClient _client;
        private readonly IMediator _mediator;

        public GameProviderReportController(IReportClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get game provider round aggregate")]
        public async Task<PagedResponse<GameProviderRoundsModel>> GetGameProviderRoundsAsync(
            [FromQuery] GetGameProviderRoundsReportRequest request)
        {
            var reportRequest = new GetGameProviderRoundsRequest
            {
                From = request.From,
                To = request.To,
                GameProviderId = request.GameProviderId,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            return await _client.GetGameProviderRoundsAsync(reportRequest).ConfigureAwait(false);
        }

        [HttpPost("generate")]
        [Description("Generate game provider round aggregate")]
        public Task<MessageResponse> GenerateGameProviderAggregateAsync(
            [FromBody] GenerateGameProviderAggregateRequest request)
            => _mediator.Send(request);
    }
}