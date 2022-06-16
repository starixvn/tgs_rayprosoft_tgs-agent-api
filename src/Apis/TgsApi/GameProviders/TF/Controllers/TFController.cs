using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.TF.Services;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TF.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TF.Controllers;

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
    private readonly IBrandOperations _brandOperations;
    private readonly ILogger<TFController> _logger;
    private readonly ITFActionModelMapper _map;
    private readonly IPlayerOperations _playerOperations;
    private readonly ITFService _service;
    private readonly ISettingOperations _settingOperations;
    private readonly ITokenProvider _tokenProvider;

    public TFController(
        IGamePlayActions actions,
        ITFService service,
        ITFActionModelMapper map,
        ITokenProvider tokenProvider,
        IPlayerOperations playerOperations,
        IBrandOperations brandOperations,
        ISettingOperations settingOperations,
        ILogger<TFController> logger)
    {
        _actions = actions ?? throw new ArgumentNullException(nameof(actions));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _map = map ?? throw new ArgumentNullException(nameof(map));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("placebet")]
    public async ValueTask BetPlacedAsync(BetPlacedRequest request)
    {
        if (request.PlaceBet == false)
        {
            throw new InvalidOperationException();
        }
        var items = HttpContext.Items;
        var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);
        var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
        var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
        var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

        var gameProviderConfigurationData = await _service
            .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
            .ConfigureAwait(false);

        var betData = await _map.BetAsync(gameProviderConfigurationData, playerCode, request);

        if (betData == null)
        {
            throw new InvalidGameActionException("");
        }

        _ = await _actions
            .PlaceBetAsync(betData)
            .ConfigureAwait(false);
    }

    [HttpPost("settle")]
    public async ValueTask BetResultedAsync(SettleRequest request)
    {
        if (request.PlaceBet == true)
            throw new InvalidOperationException();

        var items = HttpContext.Items;
        var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);
        var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
        var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
        var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

        var gameProviderConfigurationData = await _service
           .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
           .ConfigureAwait(false);

        if (request.TicketDetail.SettlementStatus == "settled" && request.TicketDetail.IsUnsettled == false)
        {
            var betData = await _map
                .SettleAsync(gameProviderConfigurationData, playerCode, request)
                .ConfigureAwait(false);

            if (betData == null)
            {
                throw new InvalidGameActionException("");
            }

            _ = await _actions
               .SettleBetAsync(betData)
               .ConfigureAwait(false);
        }
        else if (request.TicketDetail.SettlementStatus == "settled" && request.TicketDetail.IsUnsettled == true)
        {
            //廠商結算輸 改單為贏:
            //api呼叫順序: settle輸 -> settle贏
            var betDataCancelSettle = await _map
                .CancelRoundSettleAsync(gameProviderConfigurationData, playerCode, request)
                .ConfigureAwait(false);

            if (betDataCancelSettle != null)
            {
                //改單輸因會先call unsettle 故判斷不執行
                _ = await _actions
                    .CancelTransactionAsync(betDataCancelSettle)
                    .ConfigureAwait(false);
            }

            var betData = await _map
                .SettleAsync(gameProviderConfigurationData, playerCode, request)
                .ConfigureAwait(false);

            _ = await _actions
               .SettleBetAsync(betData)
               .ConfigureAwait(false);
        }
        else if (request.TicketDetail.SettlementStatus == "cancelled")
        {
            var betDataCancelSettle = await _map
                .CancelRoundSettleAsync(gameProviderConfigurationData, playerCode, request)
                .ConfigureAwait(false);
            //廠商取消整局:
            //結算贏 -> 改單取消整局 api呼叫順序: settle贏 -> unsettle -> settle取消
            //結算輸 -> 改單取消整局 api呼叫順序: settle輸 -> settle取消
            if (betDataCancelSettle != null)
            {
                _ = await _actions
                    .CancelTransactionAsync(betDataCancelSettle)
                    .ConfigureAwait(false);
            }

            var betDataCancelBet = await _map
            .CancelRoundBetAsync(gameProviderConfigurationData, playerCode, request)
            .ConfigureAwait(false);

            _ = await _actions
                .CancelTransactionAsync(betDataCancelBet)
                .ConfigureAwait(false);
        }
    }

    [HttpGet("wallet")]
    public async ValueTask<GetClientBalanceResponse> GetClientBalanceAsync([FromQuery] GetClientBalanceRequest request)
    {
        var r = await _actions
            .GetBalanceAsync(GameProviderCode, request.LoginName)
            .ConfigureAwait(false);

        return new GetClientBalanceResponse()
        {
            Balance = r.Balance
        };
    }

    [HttpPost("rollback")]
    public async ValueTask RollbackAsync(RollbackRequest request)
    {
        var items = HttpContext.Items;
        var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);
        var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
        var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
        var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

        var gameProviderConfigurationData = await _service
            .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
            .ConfigureAwait(false);

        var betData = await _map
            .RollbackBetAsync(gameProviderConfigurationData, playerCode, request)
            .ConfigureAwait(false);

        if (betData != null)
        {
            _ = await _actions
                .SettleBetAsync(betData)
                .ConfigureAwait(false);
        }
    }

    [HttpPost("unsettle")]
    public async ValueTask UnSettleAsync(UnSettleRequest request)
    {
        if (request.PlaceBet == true)
            throw new InvalidOperationException();
        var items = HttpContext.Items;
        var playerCode = items.Get(Consts.ContextItems.PlayerCode, string.Empty);
        var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
        var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
        var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

        var gameProviderConfigurationData = await _service
            .GetGameProviderConfigurationDataAsync(gameProviderId, gameProviderConfigurationId, platformType)
            .ConfigureAwait(false);

        var betDataCancelSettle = await _map
            .CancelSettleAsync(gameProviderConfigurationData, playerCode, request)
            .ConfigureAwait(false);

        if (betDataCancelSettle == null)
        {
            throw new Exception();
        }

        if (!betDataCancelSettle.Transactions[0].ExternalTransactionId.EndsWith("_bet_cancel_cancel"))
        {
            _ = await _actions
                .CancelTransactionAsync(betDataCancelSettle)
                .ConfigureAwait(false);
        }
        else
        {
            //廠商行為
            //下單->取消->撤銷取消
            //這裡實作的是撤銷取消的動作:因為原局號已經close，需要重下一張單子

            var betDataReBet = await _map.CancelSettleReBetAsync(gameProviderConfigurationData, playerCode, request);

            if (betDataReBet == null)
            {
                throw new InvalidGameActionException("");
            }

            _ = await _actions
                .CancelTransactionAsync(betDataReBet)
                .ConfigureAwait(false);
        }
    }

    [HttpPost("token/validate")]
    public async ValueTask<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request)
    {
        var playerCode = await _tokenProvider
            .DecryptAsync<string>(request.Token)
            .ConfigureAwait(false);

        return new ValidateTokenResponse
        {
            LoginName = playerCode.ToString()
        };
    }
}
