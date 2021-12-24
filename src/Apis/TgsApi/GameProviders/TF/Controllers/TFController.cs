using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.TF.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.GameOperations;
using System.Linq;
using TGS.Shared.Game.Models.TF.Enums;
using TGS.Shared.Game.Models.TF.Incoming;
using Tgs.Core.Infrastructures.Security;

namespace TGS.Apis.TgsApi.GameProviders.TF.Controllers
{
    [Route("tf/api")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.TFGaming)]
    public class TFController : ControllerBase
    {
        private static readonly string GameProviderCode = Consts.GameProvider.TFGaming;

        private readonly IGamePlayActions _actions;
        private readonly ITFService _service;
        private readonly ITFActionModelMapper _map;
        private readonly ITokenProvider _tokenProvider;

        public TFController(
            IGamePlayActions actions,
            ITFService service,
            ITFActionModelMapper map,
            ITokenProvider tokenProvider)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        [HttpPost("token/validate")]
        public async ValueTask<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request) 
        {
            var playerCode =  await _tokenProvider
                .DecryptAsync<Guid>(request.Token)
                .ConfigureAwait(false);

            return new ValidateTokenResponse
            {
                LoginName = playerCode.ToString()
            };
        }


        [HttpGet("wallet")]
        public async ValueTask<GetClientBalanceResponse> GetClientBalanceAsync(GetClientBalanceRequest request)
        {            
            var playerCode = HttpContext.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);

            var r = await _actions
                .GetBalanceAsync(GameProviderCode, playerCode)
                .ConfigureAwait(false);

            return new GetClientBalanceResponse()
            {
                Balance = r.Balance                
            };
        }

        [HttpPost("rollback")]
        public async ValueTask<RollbackResponse> RollbackAsync(RollbackRequest request)
        {            
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _service
               .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
               .ConfigureAwait(false);

            //var betData = await _map
            //    .RollbackBetAsync(gameProviderConfigurationData, playerCode, request)
            //    .ConfigureAwait(false);

            //if (betData == null)
            //{
            //    return new RollbackResponse()
            //    {
            //        ErrorCode = $"{(int)GameApiError.InternalError}",
            //        ErrorText = GameApiError.InternalError.Description()
            //    };
            //}

            //_ = await _actions.SettleBetAsync(betData).ConfigureAwait(false);

            //return new RollbackResponse()
            //{
            //    ErrorCode = $"{(int)GameApiError.Success}",
            //    ErrorText = GameApiError.Success.Description()
            //};
            return null;
        }

        [HttpPost("placebet")]
        public async ValueTask<BetPlacedResponse> BetPlacedAsync(BetPlacedRequest request)
        {            
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = request.TicketDetail.First().RequestSource switch
            {
                "desktop-browser" => PlatformType.Desktop,
                "mobile-browser" => PlatformType.Mobile,
                "mobile-app" => PlatformType.Mobile,
                "unknown" => PlatformType.Desktop,
                _ => PlatformType.Desktop
            };


            var gameProviderConfigurationData = await _service
                .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
                .ConfigureAwait(false);

            //var betData = await _map.BetAsync(gameProviderConfigurationData, playerCode, request);

            //if (betData == null)
            //{
            //    return new BetPlacedResponse()
            //    {
            //        ErrorCode = $"{(int)GameApiError.InternalError}",
            //        ErrorText = GameApiError.InternalError.Description()
            //    };
            //}

            //_ = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);

            //return new BetPlacedResponse()
            //{
            //    ErrorCode = $"{(int)GameApiError.Success}",
            //    ErrorText = GameApiError.Success.Description()
            //};
            return null;
        }

        [HttpPost("settlement")]
        public async ValueTask<BetResultedResponse> BetResultedAsync(BetResultedRequest request)
        {           
            var items = HttpContext.Items;
            var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var gameProviderConfigurationData = await _service
               .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
               .ConfigureAwait(false);

            //if (request.IsResettled == false)
            //{
            //    var betData = await _map.SettleAsync(gameProviderConfigurationData, playerCode, request).ConfigureAwait(false);

            //    if (betData == null)
            //    {
            //        return new BetResultedResponse()
            //        {
            //            ErrorCode = $"{(int)GameApiError.InternalError}",
            //            ErrorText = GameApiError.InternalError.Description()
            //        };
            //    }

            //    var betResult = await _actions.SettleBetAsync(betData).ConfigureAwait(false);
            //}
            //else
            //{
            //    var betDataCancelSettle = await _map.CancelSettleAsync(gameProviderConfigurationData, playerCode, request).ConfigureAwait(false);
            //    if (betDataCancelSettle == null)
            //    {
            //        return new BetResultedResponse()
            //        {
            //            ErrorCode = $"{(int)GameApiError.InternalError}",
            //            ErrorText = GameApiError.InternalError.Description()
            //        };
            //    }
            //    var betResultCancelSettle = await _actions.CancelTransactionAsync(betDataCancelSettle).ConfigureAwait(false);

            //    var betDataSettle = await _map.ReSettleAsync(gameProviderConfigurationData, playerCode, request).ConfigureAwait(false);

            //    if (betDataSettle == null)
            //    {
            //        return new BetResultedResponse()
            //        {
            //            ErrorCode = $"{(int)GameApiError.InternalError}",
            //            ErrorText = GameApiError.InternalError.Description()
            //        };
            //    }

            //    var betResult = await _actions.SettleBetAsync(betDataSettle).ConfigureAwait(false);
            //}

            //return new BetResultedResponse()
            //{
            //    ErrorCode = $"{(int)GameApiError.Success}",
            //    ErrorText = GameApiError.Success.Description()
            //};
            return null;
        }

        [HttpPost("unsettlement")]
        public ValueTask UnSettleAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
