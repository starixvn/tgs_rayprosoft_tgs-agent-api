using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AnnouncementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnnouncementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("announcement/list")]
        public Task<GetAnnouncementsResponse> GetAnnouncementsAsync() =>
            _mediator.Send(new GetAnnouncementsRequest());
    }
}
