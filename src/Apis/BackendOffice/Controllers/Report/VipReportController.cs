using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Report.Client;
using Report.Client.Models.Vip;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public VipReportController(IReportClient client)
        {
            _client = client;
        }

        [HttpGet]
        [Description("Get vips")]
        public Task<PagedResponse<VipModel>> GetVipsAsync([FromQuery] GetVipReportRequest request) =>
            _client.GetVipReportAsync(request);

        [HttpGet("{username}")]
        [Description("Get vip rebates")]
        public Task<PagedResponse<VipRebateModel>> GetVipRebatesAsync(string username,
            [FromQuery] GetVipRebateReportRequest request)
        {
            request.Username = username;
            return _client.GetVipRebateReportAsync(request);
        }
    }
}