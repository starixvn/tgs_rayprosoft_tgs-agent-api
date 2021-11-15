using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Round;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Core.Models.Messages;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.ViewModels;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : BaseContextController
    {
        private readonly IReportClient _client;
        private readonly IGameProvidersPluggableClient _gameRoundClient;

        public RoundController(IReportClient client, IGameProvidersPluggableClient gameRoundClient)
        {
            _client = client;
            _gameRoundClient = gameRoundClient;
        }

        [HttpGet("openRounds")]
        [Description("Get open rounds")]
        public Task<PagedResponse<OpenRoundModel>> GetOpenRoundsAsync([FromQuery] GetOpenRoundsRequest request) =>
            _client.GetOpenRoundsAsync(request);

        [HttpPut("{roundId}/CloseGameRound")]
        [Description("Close a game round")]
        public async Task<MessageResponse> CloseGameRoundAsync(Guid roundId,
            [FromQuery] CloseGameRoundRequest request)
        {
            var model = new GameRoundClosedViewModel
            {
                BetAmount = request.BetAmount,
                ClientRoundStatus = request.ClientRoundStatus,
                Commission = request.Commission,
                ExternalClosedOn = request.ExternalClosedOn,
                OperatorId = Guid.Parse(UserId),
                Remark = request.Remark,
                SettleAmount = request.SettleAmount,
                Turnover = request.Turnover,
                ValidBetAmount = request.ValidBetAmount,
                WinLoss = request.WinLoss
            };

            await _gameRoundClient.GameRoundClosedAsync(roundId, model).ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}