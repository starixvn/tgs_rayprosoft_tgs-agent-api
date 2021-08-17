using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.WMCasino.Enums;
using TGS.Shared.Game.Models.WMCasino.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Controllers
{
    [Route("api/wm")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.WMCasino)]
    public class WMCasinoController : ControllerBase
    {
        private static readonly string _GameProviderCode = Consts.GameProvider.WMCasino;

        private readonly IGamePlayActions _actions;
        private readonly IWMCasinoActionModelMapper _map;
        private readonly IWMCasinoServices _services;

        public WMCasinoController(
            IGamePlayActions actions,
            IWMCasinoServices services,
            IWMCasinoActionModelMapper map)
        {
            _actions = actions;
            _services = services;
            _map = map;
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<ErrorResponse> Entrance([FromForm] BaseRequest request)
            => ValueTask.FromResult(new ErrorResponse());

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("balance")]
        public async ValueTask<GetBalanceResponse> PlayersBalanceAsync(GetBalanceRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return _map.Response(request.User, r);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("transfer")]
        public async ValueTask<WalletTransferResponse> WalletTransferAsync(WalletTransferRequest request)
        {
            if (request.ActionType == WalletActionType.Debit)
            {
                return await LockAmountAsync(request).ConfigureAwait(false);
            }

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("canceltransfer")]
        public async ValueTask<CancelWalletTransferResponse> CancelWalletTransferAsync(
            CancelWalletTransferRequest request)
        {
            if (request.ActionType == WalletActionType.Debit)
            {
                return await CancelLockAmountAsync(request).ConfigureAwait(false);
            }

            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var r = await _services.GetBalanceAsync(playerCode).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        private async ValueTask<WalletTransferResponse> LockAmountAsync(WalletTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync(playerCode, request.ExternalGameId)
                .ConfigureAwait(false);

            var betData = _map.LockAmountRequest(playerCode, data, request);
            var r = await _actions.LockAmountAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }

        private async ValueTask<CancelWalletTransferResponse> CancelLockAmountAsync(CancelWalletTransferRequest request)
        {
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var data = await _services.GetGameProviderConfigurationDataAsync(playerCode, request.ExternalGameId)
                .ConfigureAwait(false);

            var betData = _map.CancelLockAmountRequest(playerCode, data, request);
            var r = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            return _map.Response(request, r.Balance);
        }
    }
}