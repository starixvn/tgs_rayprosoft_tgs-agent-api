using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
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

        [HttpPut("{id}")]
        [Description("Update a promotion")]
        public Task<MessageResponse> UpdatePromotionAsync(Guid id, UpdatePromotionRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }
    }
}