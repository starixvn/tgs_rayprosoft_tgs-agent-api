using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    PlayerCode = p.PlayerCode,
                    Username = p.Username,
                    VipLevel = p.Vip.Name,
                    Status = p.Status.ToString(),
                    HasAgent = p.HasAgent
                }).CreatePagedResponseAsync(request);
        }

        [Route("{playerCode}")]
        [HttpGet, Description("Get player")]
        public async Task<GetPlayerResponse> GetSingleAsync(string playerCode)
        {
            var player = await _queries.GetSingleAsync(p => p.PlayerCode == playerCode, p =>
                p.Include(s => s.Vip)
                    .Include(s => s.Banks)
                    .Include(s => s.Wallets)
                    .Include(s => s.ParentPlayer));

            if (player == null)
            {
                return CreateInvalidResponse<GetPlayerResponse>(Resources.Player.NotFound);
            }

            var playerModel = new PlayerModel
            {
                Username = player.Username,
                Realname = player.RealName,
                VipLevel = player.Vip.Name,
                Status = player.Status.ToString(),
                HasAgent = player.HasAgent,
                RegisteredOn = player.RegisteredOn,
                ParentUsername = player.ParentPlayer.Username,
                Balance = player.Wallets.FirstOrDefault() == null ? 0 : player.Wallets.FirstOrDefault().Balance,
                Phone = player.Phone,
                Email = player.Email,
                QQ = player.QQ,
                WeChat = player.Wechat,
                Banks = player.Banks.Select(p => new PlayerModel.BankModel
                {
                    RealName = p.Username,
                    BankName = p.BankName,
                    BankAccount = p.BankAccount
                }).ToArray()

            };

            return new GetPlayerResponse
            {
                Valid = true,
                Player = playerModel
            };
        }
    }
}
