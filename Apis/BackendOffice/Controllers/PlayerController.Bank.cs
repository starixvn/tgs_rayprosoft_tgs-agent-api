using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{
    public partial class PlayerController
    {

        [Route("")]
        [HttpPut("{id}"), Description("Get player list")]
        public Task<PagedResponse<PlayersModel>> UadateBankAsync([FromQuery] GetPlayersRequest request)
        {
            return _queries.GetQueries()
                .Where(request.Status, p => p.Status == request.Status)
                .Where(request.VipId, p => p.VipId == request.VipId)
                .Where(request.Username, p => request.Username.Contains(p.Username))
                .Select(p => new PlayersModel
                {
                    PlayerCode = p.PlayerCode,
                    Username = p.Username,
                    VipLevel = p.Vip.Name,
                    Status = p.Status.ToString(),
                    HasAgent = p.HasAgent
                }).CreatePagedResponseAsync(request);
        }

    }
}
