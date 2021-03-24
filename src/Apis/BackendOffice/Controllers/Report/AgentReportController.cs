using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentReportController : BaseContextController
    {
        private readonly IMediator _mediator;

        public AgentReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("contribution")]
        [Description("Get agent contributions")]
        public Task<PagedResponse<AgentContributionModel>> GetRoundsAsync(
            [FromQuery] GetAgentContributionsRequest request) =>
            _mediator.Send(request);
    }
}