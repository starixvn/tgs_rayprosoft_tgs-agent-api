using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class PluggableController : ControllerBase
    {
        [HttpPost]
        [Route("gameurl")]
        public async ValueTask<GetGameUrlResponse> GetGameUrlAsync(
            [FromServices] Func<string, IGameInitializer> initializerFactory,
            GetGameUrlRequest request) =>
            await initializerFactory(request.GameProviderCode).GetGameUrlAsync(request)
                .ConfigureAwait(false);

        [HttpPost]
        [Route("history/round")]
        public async ValueTask<GetRoundHistoryResponse> GetRoundHistory(
            [FromServices] Func<string, IGameHistory> historyFactory,
            GetRoundHistoryRequest request) =>
            await historyFactory(request.GameProviderCode).GetRoundHistoryAsync(request)
                .ConfigureAwait(false);

        [HttpPost]
        [Route("player/balance")]
        public async ValueTask<GameProviderWalletGetBalanceResponse> GetBalanceAsync(
            [FromServices] Func<string, IGameProviderWalletAction> walletActionFactory,
            GameProviderWalletGetBalanceRequest request) =>
            await walletActionFactory(request.GameProviderCode).GetBalanceAsync(request)
        .ConfigureAwait(false);

        [HttpPost]
        [Route("player/fundout")]
        public async ValueTask<GameProviderWalletFundResponse> FundOutPlayerAsync(
            [FromServices] Func<string, IGameProviderWalletAction> walletActionFactory,
            GameProviderWalletFundRequest request) =>
            await walletActionFactory(request.GameProviderCode).FundOutAsync(request)
            .ConfigureAwait(false);

        [HttpPost]
        [Route("player/fundin")]
        public async ValueTask<GameProviderWalletFundResponse> FundInPlayerAsync(
            [FromServices] Func<string, IGameProviderWalletAction> walletActionFactory,
            GameProviderWalletFundRequest request) =>
            await walletActionFactory(request.GameProviderCode).FundInAsync(request)
            .ConfigureAwait(false);

        [HttpPost]
        [Route("player/kick")]
        public async ValueTask<PlayerKickResponse> KickPlayerAsync(
            [FromServices] IGameProviderOperations gameProviderOperations,
            [FromServices] Func<string, IPlayerAction> playerActionFactory,
            PlayerKickRequest request)
        {
            var gameProviderCode = request.GameProviderCode;
            var gameProviderId = await gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);
            var allowKickPlayer = await gameProviderOperations.GetAllowKickPlayerAsync(gameProviderId);

            if (!allowKickPlayer)
            {
                throw new NotSupportedException(
                    $"Game Provider '{request.GameProviderCode}' doesn't support to kick player");
            }

            return await playerActionFactory(gameProviderCode).KickAsync(request);
        }
    }
}