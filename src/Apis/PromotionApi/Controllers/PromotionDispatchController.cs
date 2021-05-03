using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.PromotionCore.Models.PromotionDispatch;

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
    }
}