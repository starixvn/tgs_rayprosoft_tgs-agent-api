using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Enums;

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
        [HttpGet]
        [Route("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public Task<GetGamesResponse> GetGamesAsync([FromQuery] GetGamesRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     get game launch url via game id
        /// </summary>
        [HttpGet("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetGameUrlForGetAsync(
            [FromQuery(Name = "gpcode")] string gpCode,
            [FromQuery(Name = "gid")] int gameId,
            [FromQuery(Name = "language")] string languageCode
            ) =>
            _mediator.Send(new GetGameUrlRequest()
            {
                GameId = gameId,
                LanguageCode = languageCode,
                GameProviderCode = gpCode
            });

        /// <summary>
        ///     get game launch url via game id
        /// </summary>
        [HttpPost("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetGameUrlForPostAsync([FromBody] GetGameUrlRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     get game launch url for lobby type game.
        /// </summary>
        [HttpGet("lobby/geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetLobbyGameUrlForGetAsync(
            [FromQuery(Name = "gpcode")] string gpCode,
            [FromQuery(Name = "gtype")] GameType gameType,
            [FromQuery(Name = "language")] string languageCode) =>
            _mediator.Send(new GetLobbyGameUrlRequest
            {
                GameProviderCode = gpCode,
                GameType = gameType,
                LanguageCode = languageCode
            });

        /// <summary>
        ///     get game launch url for lobby type game.
        /// </summary>
        [HttpPost("lobby/geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetGameUrlResponse> GetLobbyGameUrlForPostAsync([FromBody] GetLobbyGameUrlRequest request) =>
            _mediator.Send(request);
    }
}