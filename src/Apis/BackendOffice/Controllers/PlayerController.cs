using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Promotion.Client;
using TGS.Clients.Promotion.Client.Models.Promotion;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Player;
using TGS.Services.BackOffice.Application.Models.Player;
using TGS.Services.BackOffice.Application.Models.ReportPlayer;
using Tgs.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
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

        [HttpPut("{playerCode}/clear")]
        [Description("Clear player buckets")]
        public Task<ClearPlayerBucketsResponse> ClearPlayerBucketsAsync(string playerCode) =>
            _mediator.Send(new ClearPlayerBucketsRequest(playerCode));

        [HttpPost("{playerCode}/adjust/increase")]
        [Description("Create a player adjustment to increase amount")]
        public Task<MessageResponse> CreatePlayerIncreaseAmountAsync(string playerCode,
            CreatePlayerIncreaseAmountRequest request)
        {
            request.PlayerCode = playerCode;
            return _mediator.Send(request);
        }

        [HttpPost("{playerCode}/adjust/reduce")]
        [Description("Create a player adjustment to reduce amount")]
        public Task<MessageResponse> CreatePlayerReduceAmountAsync(string playerCode,
            CreatePlayerReduceAmountRequest request)
        {
            request.PlayerCode = playerCode;
            return _mediator.Send(request);
        }

        [HttpGet("accountChanges")]
        [Description("Get player account changes")]
        public async Task<PagedResponse<PlayerAccountChangeReportModel>?> GetAccountChangesAsync(
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
                    RecordedOn = x.RecordedOn
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

        [HttpGet("{playerCode}/joinedPromotions")]
        [Description("Get player operation")]
        public Task<PagedResponse<JoinedPromotionModel>> GetJoinedPromotionsAsync(
            string playerCode,
            [FromQuery] GetJoinedPromotionsRequest request)
        {
            request.PlayerCode = playerCode;
            return _client.GetJoinedPromotionsAsync(request);
        }

        [HttpGet]
        [Description("Get player list")]
        public Task<PagedResponse<PlayersModel>> GetListAsync([FromQuery] GetPlayersRequest request)
            => _mediator.Send(request);

        [HttpGet("{playerCode}/transferwallet")]
        [Description("Get Player Transfer Wallet")]
        public Task<GetPlayerGameProviderBalanceResponse[]> GetPlayerGameProviderBalancesAsync(string playerCode)
            => _mediator.Send(new GetPlayerGameProviderBalanceRequest(playerCode));

        [HttpGet("{playerCode}/mainwallet")]
        [Description("Get Player Main wallet")]
        public ValueTask<decimal> GetPlayerMainWalletBalanceAsync(
            [FromServices] IGetService<GetPlayerMainWalletBalance, decimal> service,
            string playerCode)
            => service.GetAsync(new(playerCode));

        [HttpGet("{playerCode}/operations")]
        [Description("Get player operation")]
        public Task<PagedResponse<PlayerOperationModel>> GetPlayerOperationsAsync(string playerCode,
            [FromQuery] GetPlayerOperationRequest request)
        {
            request.PlayerCode = playerCode;
            return _mediator.Send(request);
        }

        [HttpGet("{playerCode}")]
        [Description("Get player")]
        public Task<GetPlayerResponse> GetSingleAsync(string playerCode) =>
            _mediator.Send(new GetPlayerRequest(playerCode));

        [HttpPut("{playerCode}/{type}")]
        [Description("Update player")]
        public Task<MessageResponse> UpdateAsync(string playerCode, string type,
            [FromBody] UpdatePlayerRequest request)
        {
            request.PlayerCode = playerCode;
            request.Type = type;
            return _mediator.Send(request);
        }
    }
}