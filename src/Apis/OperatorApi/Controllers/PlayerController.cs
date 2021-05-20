using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.ServiceUtility;
using TGS.Services.Operator.Player;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public partial class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Check username exists
        /// </summary>
        [HttpGet]
        [Route("checkUsername")]
        public Task<CheckUsernameResponse> CheckUsernameAsync([FromQuery] CheckUsernameRequest request) 
            => _mediator.Send(request);

        /// <summary>
        /// Create a player
        /// </summary>
        [HttpPost]
        [Route("create")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request) =>
            _mediator.Send(request);

        /// <summary>
        /// Player login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);

        /// <summary>
        /// Player logout, player login is needed when use this
        /// </summary>
        [HttpDelete]
        [Route("logout")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<LogoutResponse> LogoutAsync() =>
            _mediator.Send(new LogoutRequest());

        /// <summary>
        /// Get player information
        /// </summary>
        [HttpGet]
        [Route("info")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetPlayerResponse> GetPlayerAsync() =>
            _mediator.Send(new GetPlayerRequest());

        /// <summary>
        /// Sets the qq number.
        /// </summary>
        /// <param name="setQqService">The set qq service.</param>
        /// <param name="request">The request.</param>
        [HttpPatch("qq")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetQqIdAsync(
            [FromServices] ICommandService<SetQqCommand> setQqService,
            [FromBody] SetQqIdRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetQqCommand(playerTokenData.PlayerCode, request.QqId);

            await setQqService.ExecuteAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the we chat identifier.
        /// </summary>
        /// <param name="setWeChatService">The set we chat service.</param>
        /// <param name="request">The request.</param>
        [HttpPatch("wechat")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetWeChatIdAsync(
            [FromServices] ICommandService<SetWeChatCommand> setWeChatService,
            [FromBody] SetWeChatIdRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetWeChatCommand(playerTokenData.PlayerCode, request.WebChatId);

            await setWeChatService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
