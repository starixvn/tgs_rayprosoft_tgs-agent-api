using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Vip;

namespace TGS.Apis.ReportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VipReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get vips")]
        public Task<PagedResponse<VipModel>> GetVipsAsync([FromQuery] GetVipReportRequest request) =>
            _mediator.Send(request);

        [HttpGet("{username}")]
        [Description("Get vip rebates")]
        public Task<PagedResponse<VipRebateModel>> GetVipRebatesAsync(string username,
            [FromQuery] GetVipRebateReportRequest request)
        {
            request.Username = username;
            return _mediator.Send(request);
        }
    }
}