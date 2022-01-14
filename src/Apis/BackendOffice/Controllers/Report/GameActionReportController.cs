using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.GameAction;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameActionReportController : BaseContextController
    {
        [HttpGet]
        [Description("Get game actions")]
        public Task<PagedResponse<GameActionModel>> GetGameActionsAsync(
            [FromServices] IReportClient client,
            [FromQuery] GetGameActionsRequest request)
            => client.GetGameActionsAsync(request);
    }
}