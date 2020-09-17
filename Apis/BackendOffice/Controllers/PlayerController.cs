using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Players;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayerController : BaseUserContextController
    {
        private readonly ITgsQueries<Player> _queries;
        private readonly ITgsCommands<Player> _commands;
        private readonly ITgsCommands<PlayerBank> _bankCommands;
        private readonly IBackendOfficeCommands<AuditLog> _auditLogCommands;
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _tgsHashConfig;

        public PlayerController(
            IHashProvider hashProvider,
            ITgsHashConfig tgsHashConfig,
            ITgsQueries<Player> queries,
            ITgsCommands<Player> commands,
            ITgsCommands<PlayerBank> bankCommands,
            IBackendOfficeCommands<AuditLog> auditLogCommands,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _hashProvider = hashProvider;
            _tgsHashConfig = tgsHashConfig;
            _queries = queries;
            _commands = commands;
            _bankCommands = bankCommands;
            _auditLogCommands = auditLogCommands;
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
                    Id = p.Id,
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

        [Route("{playerCode}/operation")]
        [HttpGet, Description("Get player operation")]
        public async Task<GetPlayerResponse> GetPlayerOperationsAsync(string playerCode)
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
                    Id = p.Id,
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

        [Route("{playerCode}/{type}")]
        [HttpPut, Description("Update player")]
        public async Task<MessageResponse> UpdateAsync(string playerCode, string type, [FromBody] UpdatePlayerRequest request)
        {
            var updatePlayerType = new[] { "Info", "Contact", "Password" };

            if (!updatePlayerType.Contains(type))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.UpdateTypeNotFound);
            }

            var player = await _queries.GetSingleAsync(p => p.PlayerCode == playerCode);

            if (player == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.NotFound);
            }



            if (type == updatePlayerType[0])
            {
                var parentPlayer = await _queries.GetSingleAsync(p => p.PlayerCode == request.ParentUsername);

                if (parentPlayer == null)
                {
                    return CreateInvalidResponse<MessageResponse>(Resources.Player.ParentPlayerNotFound);
                }

                player.RealName = request.RealName;
                player.VipId = request.VipId;
                player.ParentPlayerCode = parentPlayer.ParentPlayerCode;
                player.Status = request.Status;
                player.HasAgent = request.HasAgent;

            }

            if (type == updatePlayerType[1])
            {
                player.Phone = request.Phone;
                player.Email = request.Email;
                player.QQ = request.QQ;
                player.Wechat = request.Wechat;
            }

            if (type == updatePlayerType[2])
            {
                var (pwd, salt) = GenerateCredential(request.Password);
                player.Password = pwd;
                player.Salt = salt;
            }

            player.UpdatedByUsername = Username;
            player.UpdatedOn = DateTimeOffset.Now;
            await _commands.UpdateAsync(player);
            await CreateAuditLogAsync(type);
            return CreateValidResponse<MessageResponse>();
        }

        private async Task CreateAuditLogAsync(string type)
        {
            var auditLog = new AuditLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ModuleName = "Player",
                CreatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                Operation = $"Update Player {type}"
            };

            await _auditLogCommands.CreateAsync(auditLog);
        }

        protected (byte[], byte[]) GenerateCredential(string password)
        {
            var salt = _hashProvider.GenerateRandomSalt(32);
            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                _hashProvider.Hash(salt),
                _tgsHashConfig.HashAlgorithm);
            return (salt, hashedPassword);
        }
    }
}
