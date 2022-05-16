using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using Tgs.Promotion.Client;
using Tgs.Promotion.Client.Models.PromotionDispatch;

namespace TGS.Apis.BackendOffice.Controllers.Promotion
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionDispatchController : BaseContextController
    {
        private readonly IPromotionDispatchClient _client;

        public PromotionDispatchController(IPromotionDispatchClient client)
        {
            _client = client;
        }

        [HttpGet(Name = "GetPromotions")]
        [Description("Get promotion dispatches")]
        public async Task<PagedResponse<PromotionDispatchModel>> GetPromotionsAsync(
            [FromQuery] GetPromotionDispatchesRequest request)
        {
            var response = await _client.GetPromotionDispatchesAsync(request);

            return new PagedResponse<PromotionDispatchModel>
            {
                Valid = response.Valid, 
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items
            };
        }

        [HttpPost(Name = "DispatchPromotions")]
        [Description("Dispatch promotion")]
        public async Task<MessageResponse> DispatchPromotionsAsync(
            [FromBody] DispatchPromotionBonusRequest request)
        {
            request.OperatorUserId = Guid.Parse(UserId);

            var response = await _client.DispatchPromotionAsync(request);

            return new MessageResponse { Valid = response.Valid, Message = response.Message };
        }
    }
}