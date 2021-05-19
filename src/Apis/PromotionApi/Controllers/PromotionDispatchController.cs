using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.PromotionDispatch;

namespace PromotionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionDispatchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionDispatchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get promotion dispatches")]
        public Task<PagedResponse<PromotionDispatchModel>> GetPromotionsAsync(
            [FromQuery] GetPromotionDispatchesRequest request) =>
            _mediator.Send(request);

        [HttpPost]
        [Description("Dispatch promotion")]
        public Task<MessageResponse> DispatchPromotionsAsync(
            [FromBody] DispatchPromotionBonusRequest request) =>
            _mediator.Send(request);
    }
}