using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Round;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : BaseContextController
    {
        private readonly IMediator _mediator;

        public RoundController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get rounds")]
        public Task<PagedResponse<RoundModel>> GetRoundsAsync([FromQuery] GetRoundsRequest request) =>
            _mediator.Send(request);
    }
}