using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.PromotionApi.ViewModels;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.Promotion.Models.Promotion;
using TGS.Services.Promotion.Services;
using TGS.ServiceUtility;

namespace TGS.Apis.PromotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{promotionId}/addPlayer/{playerCode}")]
        [Description("Add player to promotion")]
        public Task<MessageResponse> AddPlayerToPromotionAsync(Guid promotionId, string playerCode) =>
            _mediator.Send(new CreatePromotionParticipantRequest(promotionId, playerCode));

        [HttpPut("{promotionId}/{status}")]
        [Description("Change promotion status")]
        public Task<MessageResponse> ChangePromotionStatusAsync(Guid promotionId, SwitchStatus status) =>
            _mediator.Send(new ChangePromotionStatusRequest(promotionId, status));

        [HttpGet("{promotionId}/check/{brandId}/{playerCode}/{amount}")]
        public async Task<bool> CheckCanJoinPromotionAsync(
            [FromServices] IGetService<CheckCanJoinPromotionQuery, bool> checkService,
            Guid promotionId,
            Guid brandId,
            string playerCode,
            decimal amount)
        {
            var query = new CheckCanJoinPromotionQuery(
                brandId,
                playerCode,
                promotionId,
                amount);

            var result = await checkService.GetAsync(query).ConfigureAwait(false);

            return result;
        }

        [HttpPost]
        [Description("Create a promotion")]
        public Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request) =>
            _mediator.Send(request);

        [HttpPut("deposit/{playerCode}")]
        [Description("Tag player deposit")]
        public async Task DepositAsync(
            [FromServices] ICommandService<TagPlayerDepositCommand> tagService,
            string playerCode)
        {
            var command = new TagPlayerDepositCommand(playerCode);

            await tagService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpGet]
        [Description("Get promotions")]
        public Task<PagedResponse<PromotionModel>> GetPromotionsAsync([FromQuery] GetPromotionsRequest request) =>
            _mediator.Send(request);

        [HttpGet("valid")]
        [Description("Get promotions fro player")]
        public async Task<PagedResponse<PromotionModel>> GetPromotionsForPlayerAsync(
            [FromServices] IQueryService<GetValidPromotionsQuery, PromotionModel> queryAllService,
            [FromServices] IQueryService<GetValidPromotionsByPlayerQuery, PromotionModel> queryByPlayerService,
            [FromQuery] GetValidPromotionsRequest request)
        {
            var promotions = string.IsNullOrWhiteSpace(request.PlayerCode)
                ? queryAllService.QueryAsync(new GetValidPromotionsQuery(request.BrandId))
                : queryByPlayerService.QueryAsync(new GetValidPromotionsByPlayerQuery(
                    request.BrandId,
                    request.PlayerCode));

            var paging = new Paging<PromotionModel>();

            if (!string.IsNullOrWhiteSpace(request.SortBy))
                paging.SortByPropertyName(request.SortBy, request.IsDesc);

            if (!request.ShowAll)
                return new PagedResponse<PromotionModel>
                {
                    TotalCount = await promotions.CountAsync().ConfigureAwait(false),
                    Items = await promotions
                        .ApplyOrderBy(paging)
                        .ToArrayAsync()
                        .ConfigureAwait(false)
                };
            paging.Page = request.Page;
            paging.RowCount = request.Rows;

            return new PagedResponse<PromotionModel>
            {
                TotalCount = await promotions.CountAsync().ConfigureAwait(false),
                Items = await promotions
                    .ApplyOrderBy(paging)
                    .ApplyPaging(paging)
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };
        }

        [HttpDelete("{promotionId}/{amount}")]
        public async Task RevertPromotionAsync(
            [FromServices] ICommandService<RevertPromotionAmountCommand> revertService,
            Guid promotionId,
            decimal amount)
        {
            if (amount <= 0)
            {
                return;
            }

            var command = new RevertPromotionAmountCommand(promotionId, amount);

            await revertService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpPut("{id}")]
        [Description("Update a promotion")]
        public Task<MessageResponse> UpdatePromotionAsync(Guid id, UpdatePromotionRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }

        [HttpGet("joined")]
        [Description("Get joined promotions")]
        public Task<PagedResponse<JoinedPromotionModel>> GetPromotionsAsync(
            [FromQuery] GetJoinedPromotionsRequest request) =>
            _mediator.Send(request);
    }
}