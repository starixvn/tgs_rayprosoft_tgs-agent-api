using System;
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
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Check username exists
        /// </summary>
        [HttpGet]
        [Route("checkUsername")]
        public Task<CheckUsernameResponse> CheckUsernameAsync([FromQuery] CheckUsernameRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     Create a player
        /// </summary>
        [HttpPost]
        [Route("create")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     Player login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     Player logout, player login is needed when use this
        /// </summary>
        [HttpDelete]
        [Route("logout")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<LogoutResponse> LogoutAsync() =>
            _mediator.Send(new LogoutRequest());

        /// <summary>
        ///     Get player information
        /// </summary>
        [HttpGet]
        [Route("info")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetPlayerResponse> GetPlayerAsync() =>
            _mediator.Send(new GetPlayerRequest());

        /// <summary>
        ///     Player forget password
        /// </summary>
        [HttpPost]
        [Route("forgotPassword")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task ForgotPasswordAsync(ForgotPasswordRequest request) => throw new NotImplementedException();

        /// <summary>
        ///     Update player information, player login is needed when use this
        /// </summary>
        [HttpPut]
        [Route("update")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<UpdatePlayerResponse> UpdatePlayerAsync(UpdatePlayerRequest request) =>
            _mediator.Send(request);
    }
}