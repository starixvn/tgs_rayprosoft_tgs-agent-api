using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.Promotion.Clients;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Player;
using Tgs.ServiceUtility;
using TGS.Apis.BackendOffice.Controllers.Messages.Promotion;
using TGS.Apis.BackendOffice.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Models.PlayerBucket;
using TGS.Services.BackOffice.Application.Models.ReportPlayer;
using TGS.Services.BackOffice.Application.Queries.PlayerHandlers;

namespace TGS.Apis.BackendOffice.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class PlayerController : BaseContextController
{
    private readonly IPromotionClient _client;
    private readonly IMediator _mediator;

    public PlayerController(IMediator mediator, IPromotionClient client)
    {
        _mediator = mediator;
        _client = client;
    }

    [HttpPut("{playerCode}/clear", Name = "ClearPlayerBuckets")]
    [Description("Clear player buckets")]
    public Task<ClearPlayerBucketsResponse> ClearPlayerBucketsAsync(string playerCode) =>
        _mediator.Send(new ClearPlayerBucketsRequest(playerCode));

    [HttpPost("{playerCode}/adjust/increase", Name = "CreatePlayerIncreaseAmount")]
    [Description("Create a player adjustment to increase amount")]
    public Task<MessageResponse> CreatePlayerIncreaseAmountAsync(string playerCode,
        CreatePlayerIncreaseAmountRequest request)
    {
        request.PlayerCode = playerCode;
        return _mediator.Send(request);
    }

    [HttpPost("{playerCode}/adjust/reduce", Name = "CreatePlayerReduceAmount")]
    [Description("Create a player adjustment to reduce amount")]
    public Task<MessageResponse> CreatePlayerReduceAmountAsync(string playerCode,
        CreatePlayerReduceAmountRequest request)
    {
        request.PlayerCode = playerCode;
        return _mediator.Send(request);
    }

    [HttpGet("accountChanges", Name = "GetPlayerAccountChanges")]
    [Description("Get player account changes")]
    public async Task<PagedResponse<PlayerAccountChangeReportModel>> GetAccountChangesAsync(
        [FromServices] IReportClient client,
        [FromQuery] GetPlayerAccountChangesReportRequest request)
    {
        var reportRequest = new GetPlayerAccountChangesRequest
        {
            BrandId = request.BrandId,
            PlayerCode = request.PlayerCode,
            AccountChangeType = (short?)request.AccountChangeType,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        };
        var response = await client.GetPlayerAccountChangesAsync(reportRequest);

        return new()
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items?.Select(x => new PlayerAccountChangeReportModel
            {
                TransactionId = x.TransactionId,
                GameActionId = x.GameActionId,
                Amount = x.Amount,
                Balance = x.Balance,
                TransactionType = (TransactionType?)x.TransactionType,
                GameActionType = (GameActionType?)x.GameActionType,
                RecordedOn = x.RecordedOn,
                IncludeInNetProfit = x.IncludeInNetProfit,
                Remark = GetRemark((TransactionType?)x.TransactionType, x.Description)
            }).ToArray()
        };
    }

    [HttpGet("accountChanges/totalAmount", Name = "GetAccountChangesTotalAmount")]
    [Description("Get player account changes Total Amount")]
    public async Task<PlayerAccountChangesTotalModel> GetAccountChangesTotalAmountAsync(
        [FromServices] IReportClient client,
        [FromQuery] GetPlayerAccountChangesReportTotalAmountRequest request)
    {
        var reportRequest = new GetPlayerAccountChangesTotalRequest
        {
            BrandId = request.BrandId,
            PlayerCode = request.PlayerCode,
            AccountChangeType = (short?)request.AccountChangeType,
            From = request.From,
            To = request.To
        };
        var response = await client.GetPlayerAccountChangesTotalAmountAsync(reportRequest);

        return new PlayerAccountChangesTotalModel { Amount = response };
    }

    [HttpGet("{playerCode}/joinedPromotions", Name = "GetJoinedPromotions")]
    [Description("Get player operation")]
    public async Task<PagedResponse<JoinedPromotionModel>> GetJoinedPromotionsAsync(
        string playerCode,
        [FromQuery] GetJoinedPromotionsRequest request)
    {
        var promotionRequest = new Tgs.Promotion.Clients.Models.Promotion.GetJoinedPromotionsRequest
        {
            PlayerCode = playerCode,
            From = request.From,
            To = request.To,
            Page = request.Page,
            Rows = request.Rows,
            OrderBy = (short)request.OrderBy,
            SortBy = request.SortBy,
            ShowAll = request.ShowAll
        };

        var response = await _client.GetJoinedPromotionsAsync(promotionRequest);

        return new PagedResponse<JoinedPromotionModel>
        {
            Valid = response.Valid,
            Message = response.Message,
            TotalCount = response.TotalCount,
            Items = response.Items.Select(x => new JoinedPromotionModel
            {
                PromotionName = x.PromotionName,
                CreatedOn = x.CreatedOn
            }).ToArray()
        };
    }

    [HttpGet(Name = "GetPlayerList")]
    [Description("Get player list")]
    public Task<PagedResponse<PlayersModel>> GetListAsync([FromQuery] GetPlayersRequest request)
        => _mediator.Send(request);

    [HttpGet("{playerBucketId}/playerBucketHistories", Name = "GetPlayerBucketHistories")]
    [Description("Get player buckets")]
    public ValueTask<GetPlayerBucketHistoriesResponse> GetPlayerBucketHistoriesAsync(Guid playerBucketId,
        [FromServices] IGetService<GetPlayerBucketHistoriesRequest, GetPlayerBucketHistoriesResponse> service)
        => service.GetAsync(new GetPlayerBucketHistoriesRequest(playerBucketId));

    [HttpGet("{playerCode}/playerBuckets", Name = "GetPlayerBuckets")]
    [Description("Get player buckets")]
    public ValueTask<GetPlayerBucketsResponse> GetPlayerBucketsAsync(string playerCode,
        [FromServices] IGetService<GetPlayerBucketsRequest, GetPlayerBucketsResponse> service)
        => service.GetAsync(new GetPlayerBucketsRequest(playerCode));

    [HttpGet("{playerCode}/transferwallet", Name = "GetPlayerGameProviderBalances")]
    [Description("Get Player Transfer Wallet")]
    public ValueTask<GetPlayerGameProviderBalanceResponse[]> GetPlayerGameProviderBalancesAsync(string playerCode,
        [FromServices]
        IGetService<GetPlayerGameProviderBalanceCommand, GetPlayerGameProviderBalanceResponse[]> service)
        => service.GetAsync(new GetPlayerGameProviderBalanceCommand(playerCode));

    [HttpPost("{playerCode}/fundin/all", Name = "FundInAllPlayerGameProvider")]
    [Description("FundIn All Player GameProvider")]
    public async ValueTask<MessageResponse> FundInAllPlayerGameProviderAsync(
        [FromServices] ICommandService<FundInAllPlayerGameProviderCommand> service,
        string playerCode)
    {
        await service.ExecuteAsync(new FundInAllPlayerGameProviderCommand(
            PlayerCode: playerCode)).ConfigureAwait(false);

        return CreateValidResponse<MessageResponse>();
    }

    [HttpPost("{playerCode}/fundin/", Name = "FundInPlayerGameProvider")]
    [Description("FundIn Player GameProvider")]
    public async ValueTask<MessageResponse> FundInPlayerGameProviderAsync(
        [FromServices] ICommandService<FundInPlayerGameProviderCommand> service,
        string playerCode,
        FundInPlayerGameProviderModel model)
    {
        await service.ExecuteAsync(new FundInPlayerGameProviderCommand(
            PlayerCode: playerCode,
            GameProviderCode: model.GameProviderCode)).ConfigureAwait(false);

        return CreateValidResponse<MessageResponse>();
    }


    [HttpGet("{playerCode}/mainwallet", Name = "GetPlayerMainWalletBalance")]
    [Description("Get Player Main wallet")]
    public ValueTask<decimal> GetPlayerMainWalletBalanceAsync(
        [FromServices] IGetService<GetPlayerMainWalletBalance, decimal> service,
        string playerCode)
        => service.GetAsync(new(playerCode));

    [HttpGet("{playerCode}/operations", Name = "GetPlayerOperations")]
    [Description("Get player operation")]
    public Task<PagedResponse<PlayerOperationModel>> GetPlayerOperationsAsync(string playerCode,
        [FromQuery] GetPlayerOperationRequest request)
    {
        request.PlayerCode = playerCode;
        return _mediator.Send(request);
    }

    [HttpGet("{playerCode}", Name = "GetSinglePlayer")]
    [Description("Get player")]
    public Task<GetPlayerResponse> GetSingleAsync(string playerCode) =>
        _mediator.Send(new GetPlayerRequest(playerCode));

    [HttpPut("{playerCode}/{type}", Name = "UpdatePlayer")]
    [Description("Update player")]
    public Task<MessageResponse> UpdateAsync(string playerCode, string type,
        [FromBody] Messages.Player.UpdatePlayerRequest request)
    {
        var updatePlayerModel = new UpdatePlayerRequest
        {
            PlayerCode = playerCode,
            Type = type,
            AgentUsername = request.AgentUsername,
            AreaCode = request.AreaCode,
            ConfirmedPassword = request.ConfirmedPassword,
            Email = request.Email,
            FacebookMessenger = request.FacebookMessenger,
            Line = request.Line,
            ParentUsername = request.ParentUsername,
            Password = request.Password,
            Phone = request.Phone,
            QQ = request.QQ,
            RealName = request.RealName,
            Remark = request.Remark,
            Status = request.Status,
            Telegram = request.Telegram,
            Viber = request.Viber,
            VipId = request.VipId,
            Wechat = request.Wechat,
            WhatsApp = request.WhatsApp,
            Birthdate = request.Birthdate
        };

        return _mediator.Send(updatePlayerModel);
    }

    private static string GetRemark(TransactionType? transactionType, string description)
    {
        switch (transactionType)
        {
            case TransactionType.CompanyAdjust:
            case TransactionType.AgentAdjust:
            case TransactionType.SystemAdjust:
            case TransactionType.TransferAdjust:
            case TransactionType.BonusAdjust:
                _ = JsonDocument.Parse(description).RootElement.TryGetProperty("Remark", out var remark);
                return remark.GetString();

            default:
                return string.Empty;
        }
    }
}
