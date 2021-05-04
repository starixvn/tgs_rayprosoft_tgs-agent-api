using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Promotion.Client;
using TGS.Clients.Promotion.Client.Models.PromotionDispatch;
using TGS.Core.Models.Messages;

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

        [HttpGet]
        [Description("Get promotion dispatches")]
        public Task<PagedResponse<PromotionDispatchModel>> GetPromotionsAsync(
            [FromQuery] GetPromotionDispatchesRequest request) =>
            _client.GetPromotionDispatchesAsync(request);

        [HttpPost]
        [Description("Dispatch promotion")]
        public Task<MessageResponse> DispatchPromotionsAsync(
            [FromQuery] DispatchPromotionBonusRequest request) =>
            _client.DispatchPromotionAsync(request);
    }
}