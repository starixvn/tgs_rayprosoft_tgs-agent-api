using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     get game list of specific game provider
        /// </summary>
        /// <remarks>
        /// language填 languages表的culture_code ex:zh-TW
        /// 
        /// currency填 currencies表的code ex:CNY
        /// </remarks>
        [HttpGet]
        [Route("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public Task<GetGamesResponse> GetGamesAsync([FromQuery] GetGamesRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     get game launch url via game id
        /// </summary>
        [HttpGet]
        [Route("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetGameUrlAsync([FromQuery] GetGameUrlRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     get game launch url for lobby type game.
        /// </summary>
        [HttpGet]
        [Route("lobby/geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetLobbyGameUrlAsync([FromQuery] GetLobbyGameUrlRequest request) =>
            _mediator.Send(request);
    }
}