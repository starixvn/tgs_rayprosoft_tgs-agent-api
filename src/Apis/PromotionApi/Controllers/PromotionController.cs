using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;

namespace PromotionApi.Controllers
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
        [Description("Add player to promotion")]
        public Task<MessageResponse> ChangePromotionStatusAsync(Guid promotionId, SwitchStatus status) =>
            _mediator.Send(new ChangePromotionStatusRequest(promotionId, status));

        [HttpGet("{promotionId}/check/{brandId}/{playerCode}")]
        public bool CheckCanJoinPromotion(
            Guid promotionId,
            Guid brandId,
            string playerCode)
        {
            return true;
        }

        [HttpPost]
        [Description("Create a promotion")]
        public Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request) =>
            _mediator.Send(request);

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
