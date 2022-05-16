using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Promotion.Client;
using Tgs.Promotion.Client.Models.Promotion;

namespace TGS.Apis.BackendOffice.Controllers.Promotion
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : BaseContextController
    {
        private readonly IPromotionClient _client;

        public PromotionController(IPromotionClient client)
        {
            _client = client;
        }

        [HttpPut("ChangeOrders", Name = "ChangeOrder")]
        [Description("Change Promotion Display Order")]
        public async Task<MessageResponse> ChangeOrderAsync(ChangeOrderRequest request)
        {
            var response = await _client.ChangeOrderAsync(request);
            return new MessageResponse{Valid= response.Valid,Message= response.Message };
        }

        [HttpPost]
        [Description("Create a promotion")]
        public async Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request)
        {
            var response = await _client.CreatePromotionAsync(request);
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }

        [HttpGet]
        [Description("Get promotions")]
        public async Task<PagedResponse<PromotionModel>> GetPromotionsAsync([FromQuery] GetPromotionsRequest request)
        {
            var response = await _client.GetPromotionsAsync(request);
            return new PagedResponse<PromotionModel>
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items
            };
        }

        [HttpPut("{id}")]
        [Description("Update a promotion")]
        public async Task<MessageResponse> UpdatePromotionAsync(Guid id, UpdatePromotionRequest request)
        {
            request.Id = id;
            var response = await _client.UpdatePromotionAsync(request);
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }

        [HttpPut("{promotionId:guid}/{status}")]
        [Description("change a Promotion's status")]
        public async Task<MessageResponse> UpdatePromotionStatusAsync(Guid promotionId, PromotionStatus status)
        {
            var response = await _client.ChangePromotionStatusAsync(new(promotionId, (short)status));
            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }

    }
}
