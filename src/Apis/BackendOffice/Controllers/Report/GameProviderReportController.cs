using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.GameProvider;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameProviderReportController : BaseContextController
    {
        private readonly IReportClient _client;

        public GameProviderReportController(IReportClient client) => _client = client;

        [HttpGet]
        [Description("Get game provider round aggregate")]
        public Task<PagedResponse<GameProviderRoundsModel>> GetGameProviderRoundsAsync(
            [FromQuery] GetGameProviderRoundsRequest request) =>
            _client.GetGameProviderRoundsAsync(request);
    }
}