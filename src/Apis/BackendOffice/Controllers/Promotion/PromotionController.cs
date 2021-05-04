using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Promotion.Client;
using TGS.Clients.Promotion.Client.Models.Promotion;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

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

        [HttpGet]
        [Description("Get promotions")]
        public Task<PagedResponse<PromotionModel>> GetPromotionsAsync([FromQuery] GetPromotionsRequest request) =>
            _client.GetPromotionsAsync(request);

        [HttpPost]
        [Description("Create a promotion")]
        public Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request) =>
            _client.CreatePromotionAsync(request);

        [HttpPut("{id}")]
        [Description("Update a promotion")]
        public Task<MessageResponse> UpdatePromotionAsync(Guid id, UpdatePromotionRequest request)
        {
            request.Id = id;
            return _client.UpdatePromotionAsync(request);
        }

        [HttpPut("{promotionId:guid}/{status}")]
        [Description("change a Promotion's status")]
        public Task<MessageResponse> UpdatePromotionStatusAsync(Guid promotionId, SwitchStatus status) =>
            _client.ChangePromotionStatusAsync(new(promotionId, status));
    }
}