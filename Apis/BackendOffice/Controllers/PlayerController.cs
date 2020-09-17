using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.DataServices.Models;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : BaseUserContextController
    {
        private readonly ITgsQueries<Player> _queries;
        private readonly ITgsCommands<Player> _commands;

        public PlayerController(
            ITgsQueries<Player> queries,
            ITgsCommands<Player> commands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _queries = queries;
            _commands = commands;
        }

        [Route("")]
        [HttpGet, Description("Get player list")]
        public Task<PagedResponse<PlayersModel>> GetListAsync([FromQuery] GetPlayersRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Status, p => p.Status == request.Status)
                .Where(request.VipId, p => p.VipId == request.VipId)
                .Where(request.Username, p => request.Username.Contains(p.Username))
                .Select(p => new PlayersModel
                {
                    Username = p.Username,
                    VipLevel = p.Vip.Name,
                    Status = p.Status.ToString(),
                    HasAgent = p.HasAgent
                }).CreatePagedResponseAsync(request);
        }
    }
}
