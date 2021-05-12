using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Models;
using TGS.Clients.Report.Client;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentController : ControllerBase
    {
        private readonly IReportClient _client;
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator, IReportClient client)
        {
            _mediator = mediator;
            _client = client;
        }

        /// <summary>
        ///     Agent login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<LoginResponse> LoginAsync(LoginRequest request) =>
            _mediator.Send(request);

        /// <summary>
        ///     Get an agent information
        /// </summary>
        [HttpGet]
        [Route("info")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<GetAgentResponse> GetPlayerAsync() =>
            _mediator.Send(new GetAgentRequest());
    }
}