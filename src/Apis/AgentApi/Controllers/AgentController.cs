using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Models;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        ///// <summary>
        /////     Check username exists
        ///// </summary>
        //[HttpGet]
        //[Route("checkUsername")]
        //public Task<CheckUsernameResponse> CheckUsernameAsync([FromQuery] CheckUsernameRequest request) =>
        //    _mediator.Send(request);

        ///// <summary>
        /////     Create a player
        ///// </summary>
        //[HttpPost]
        //[Route("create")]
        //[Consumes(MediaTypeNames.Application.Json)]
        //public Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request) =>
        //    _mediator.Send(request);

        /// <summary>
        ///     Player login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);

        ///// <summary>
        /////     Player logout, player login is needed when use this
        ///// </summary>
        //[HttpDelete]
        //[Route("logout")]
        //[AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        //public Task<LogoutResponse> LogoutAsync() =>
        //    _mediator.Send(new LogoutRequest());

        /// <summary>
        ///     Get player information
        /// </summary>
        [HttpGet]
        [Route("info")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetAgentResponse> GetPlayerAsync() =>
            _mediator.Send(new GetAgentRequest());
    }
}