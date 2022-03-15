using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Enums;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Models.Commons;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Controllers
{
    [Route("api/dg")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.DreamGame)]
    public class DreamGameController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly IDreamGamegModelActionMapper _mapper;
        private readonly IDreamGameService _service;

        public DreamGameController(
            IGamePlayActions actions,
            IDreamGamegModelActionMapper mapper,
            IDreamGameService service)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("account/checkTransfer/{agentName}")]
        public async Task<DreamGameResponse> CheckTransferAsync(DreamGameCheckTransferRequest request)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            return new()
            {
                CodeId = (int)(
                    await _service.ExistGameActionExternalIdAsync(gameProviderId, request.Data).ConfigureAwait(false)
                    ? GameApiError.OperationSucceeded
                    : GameApiError.OperationFailed),
                Token = request.Token
            };
        }

        [HttpPost("user/getBalance/{agentName}")]
        public async ValueTask<DreamGameBalanceResponse> GetBalanceAsync(DreamGameBalanceRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.DreamGame, playerCode).ConfigureAwait(false);

            return new()
            {
                CodeId = (int)GameApiError.OperationSucceeded,
                Token = request.Token,
                Member = new()
                {
                    Username = getBalance.Username,
                    Balance = getBalance.Balance
                }
            };
        }

        [HttpPost("account/inform/{agentName}")]
        public async Task<DreamGameInformResponse> InformAsync(DreamGameInformRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.DreamGame, playerCode).ConfigureAwait(false);

            if (request.Member.Amount >= 0)
            {
                //Settle
                var cancelBetData = await _mapper.CancelSettleAsync(gameProviderId, gameProviderConfigurationId, playerCode, platformType, request).ConfigureAwait(false);
                if (cancelBetData == null)
                {
                    _ = await _actions.CancelTransactionAsync(cancelBetData).ConfigureAwait(false);
                }
                var betData = await _mapper.ResettleAsync(gameProviderId, gameProviderConfigurationId, playerCode, platformType, request).ConfigureAwait(false);

                _ = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
            }
            else
            {
                //Bet
                var betData = await _mapper.CancelBetAsync(gameProviderId, gameProviderConfigurationId, playerCode, platformType, request).ConfigureAwait(false);
                _ = await _actions.CancelTransactionAsync(betData).ConfigureAwait(false);
            }
            return new()
            {
                CodeId = (int)GameApiError.OperationSucceeded,
                Token = request.Token,
                Data = request.Data,
                Member = new()
                {
                    Username = getBalance.Username,
                    Balance = getBalance.Balance,
                    Amount = request.Member.Amount
                }
            };
        }

        [HttpPost("account/order/{agentName}")]
        public async Task<DreamGameOrderResponse> OrderAsync(DreamGameOrderRequest request)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var game = await _service.GetGameAsync().ConfigureAwait(false);
            var orderInfos = await _service.GetGameActionsByExternalRoundIdAsync(gameProviderId, request.TicketId.ToString(), game.Id).ConfigureAwait(false);
            return new()
            {
                CodeId = (int)GameApiError.OperationSucceeded,
                Token = request.Token,
                TicketId = request.TicketId,
                List = orderInfos
            };
        }

        [HttpPost("account/transfer/{agentName}")]
        public async Task<DreamGameTransferResponse> TransferAsync(DreamGameTransferRequest request)
        {
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.DreamGame, playerCode).ConfigureAwait(false);

            if (request.Member.Amount >= 0)
            {
                //Settle
                var betData = await _mapper.SettleAsync(gameProviderId, gameProviderConfigurationId, playerCode, platformType, request).ConfigureAwait(false);
                _ = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
            }
            else
            {
                //Bet
                var betData = await _mapper.BetAsync(gameProviderId, gameProviderConfigurationId, playerCode, platformType, request).ConfigureAwait(false);
                _ = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
            }
            return new()
            {
                CodeId = (int)GameApiError.OperationSucceeded,
                Token = request.Token,
                Data = request.Data,
                Member = new()
                {
                    Username = getBalance.Username,
                    Balance = getBalance.Balance,
                    Amount = request.Member.Amount
                }
            };
        }

        [HttpPost("account/unsettle/{agentName}")]
        public async Task<DreamGameUnsettleResponse> UnsettleAsync(DreamGameRequest request)
        {
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            return new()
            {
                CodeId = 0,
                Token = request.Token,
                List = await _service.GetUnsettleAsync(gameProviderId).ConfigureAwait(false)
            };
        }
    }
}
