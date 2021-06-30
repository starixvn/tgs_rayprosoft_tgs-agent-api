using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Clients.Report.Client;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.Agent;
using TGS.ServiceUtility;

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

        [HttpPost("create")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<CreateAgentResponse> CreateAgentAsync(
            [FromServices] IGetService<GetCreateAgentQuery, CreateAgentInfo> getCreateAgentQueryService,
            [FromBody] CreateAgentRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var query = new GetCreateAgentQuery()
            {
                BrandId = brandId,
                ParentAgentId = agentTokenData.AgentId,
                UserName = request.UserName,
                Password = request.Password,
                Remarks = request.Remarks
            };

            var info = await getCreateAgentQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            return new CreateAgentResponse()
            {
                IsNew = info.IsNew,
                AgentId = info.AgentId,
                Message = info.Message
            };
        }
    }
}