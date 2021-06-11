using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Skywind.Common;
using TGS.Shared.Game.Models.Skywind.Enums;
using TGS.Shared.Game.Models.Skywind.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Controllers
{
    [Route("api/sw")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.Skywind)]
    public sealed class SexyGamingController : ControllerBase
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IGamePlayActions _actions;
        private readonly ISettingOperations _settingOperations;
        public SexyGamingController(
            IGamePlayActions actions,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations)
        {

            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        [HttpPost("get_balance")]
        public async Task<SkywindSingleWalletResponse> GetBalanceAsync(SkywindBalanceRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = getBalance.Balance
            };
        }

        [HttpPost("debit")]
        public async Task<SkywindSingleWalletResponse> BetAsync(SkywindBetRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = getBalance.Balance
            };
        }

        [HttpPost("credit")]
        public async Task<SkywindSingleWalletResponse> SettleAsync(SkywindSettleRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = getBalance.Balance
            };
        }


        [HttpPost("rollback")]
        public async Task<SkywindSingleWalletResponse> RollbackBetAsync(SkywindRollbackBetRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new SkywindSingleWalletResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description(),
                Balance = getBalance.Balance
            };
        }

        [HttpPost("validate_ticket")]
        public async Task<SkywindValidateTicketResponse> ValidateTicketAsync(SkywindValidateTicketRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.BigGaming, playerCode).ConfigureAwait(false);

            return new SkywindValidateTicketResponse()
            {
                ErrorCode = (int)GameApiError.Success,
                ErrorMsg = GameApiError.Success.Description()
            };
        }

        [HttpPost("ping")]
        public async Task<SkywindSingleWalletResponse> PingAsync() => new SkywindSingleWalletResponse()
        {
            ErrorCode = (int)GameApiError.Success,
            ErrorMsg = GameApiError.Success.Description(),
        };

    }
}
