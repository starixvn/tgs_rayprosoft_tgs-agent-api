using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayerController : BaseContextController
    {
        private readonly ITgsQueries<Player> _queries;
        private readonly ITgsCommands<Player> _commands;
        private readonly ITgsCommands<PlayerLoginRecord> _playerLoginRecordCommands;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITgsCommands<PlayerAuditLog> _auditLogCommands;
        private readonly IHashProvider _hashProvider;
        private readonly ICaptchaFactory _captchaFactory;

        public PlayerController(
            IHashProvider hashProvider,
            ITgsQueries<Player> queries,
            ITgsCommands<Player> commands,
            ITgsCommands<PlayerAuditLog> auditLogCommands,
            ITgsCommands<PlayerLoginRecord> playerLoginRecordCommands,
            IPlayerOperations playerOperations,
            ICaptchaFactory captchaFactory,
            IHttpContextAccessor httpContextAccessor) : 
            base(httpContextAccessor)
        {
            _hashProvider = hashProvider;
            _queries = queries;
            _commands = commands;
            _auditLogCommands = auditLogCommands;
            _playerLoginRecordCommands = playerLoginRecordCommands;
            _playerOperations = playerOperations;
            _captchaFactory = captchaFactory;
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpGet, Route("captcha")]
        public async Task<CaptchaInfo> GetCaptcha()
        {
            var model = await _captchaFactory.CreateAsync();
            return model;
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Consumes("application/json")]
        [HttpPost, Route("checkUsername"), Description("Check username exists")]
        public async Task<CheckPlayerResponse> UsernameExistsAsync(CheckPlayerRequest request)
        {
            var licenseeId = GetLicenseeIdFromHttpContext();
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            return new CheckPlayerResponse 
            { 
                Exists = playerCode.IsNotNullOrEmpty(), 
                PlayerCode = playerCode 
            };
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Consumes("application/json")]
        [HttpPost, Route("create"), Description("Create a player")]
        public async Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request)
        {
            //var result = await _captchaFactory.VerifyAsync(request.Answer, request.Captcha);
            //if (!result.IsValid)
            //{
            //    throw new InvalidOperationException(result.Message);
            //}

            var licenseeId = GetLicenseeIdFromHttpContext();
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNotNullOrEmpty())
            {
                return new CreatePlayerResponse { IsNew = false, PlayerCode = playerCode };
            }

            var (salt, password) = _hashProvider.GenerateCredential(request.Password);

            playerCode = await _playerOperations.CreatePlayerAsync(new CreatePlayerData 
            {                
                LicenseeId = licenseeId,
                Username = request.Username,
                Salt = salt,
                Password = password,
                CultureCode = Consts.DefaultCulture,
                CurrencyCode = Consts.DefaultCurrency,
                ExternalCurrencyCode = Consts.DefaultCurrency,
                ParentInvitationCode = request.ParentInvitationCode,
                BetLimitGroup = 1,
                ParentPlayerCode = "",
                Type = request.IsTestPlayer ? PlayerTypes.OperatorTestPlayer : PlayerTypes.RealPlayer,
                HasAgent = request.HasAgent
            });

            return new CreatePlayerResponse { IsNew = true, PlayerCode = playerCode };
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost("login"), Description("login")]
        public async Task<TgsResponse> LoginAsync([FromBody] LoginRequest request)
        {
            var licenseeId = new Guid(HttpContext.Request.Headers["LicenseeId"]);
            var player = await _queries.GetSingleAsync(p => p.Username == request.Username && p.LicenseeId == licenseeId);

            if (player == null)
            {
                throw new InvalidOperationException(Resources.Player.NotFound);
            }

            if (_hashProvider.PasswordMismatch(player.Password, player.Salt, request.Password))
            {
                throw new InvalidOperationException(Resources.Player.PasswordMismatched);
            }

            if (player.Status != PlayerStatus.Enabled)
            {
                throw new InvalidOperationException(Resources.Player.StatusInvalid);
            }

            var ipAddress = string.Empty;
            if (HttpContext?.Connection.RemoteIpAddress != null)
                ipAddress = HttpContext?.Connection.RemoteIpAddress.ToString();

            var logRecord = new PlayerLoginRecord
            {
                Id = GuidExtensions.NewSequentialGuid(),
                CreatedOn = DateTimeOffset.Now,
                ExpiredOn = DateTimeOffset.Now.AddHours(12),
                IpAddress = ipAddress,
                PlayerCode = player.PlayerCode,
                Status = LoginStatus.Enabled,
                PlatformType = PlatformType.Desktop,
                LicenseeId = licenseeId
            };

            await SignInAsync(player);
            await _playerLoginRecordCommands.CreateAsync(logRecord);
            return new TgsResponse();
        }

        private async Task SignInAsync(Player player)
        {
            var claims = new List<Claim>
            {
                new Claim(Consts.UserInfo.UsernameKey, player.Username),
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = true,
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [HttpPost, Route("logout")]
        public Task LogoutAsync()
        {
            return HttpContext?.SignOutAsync();
        }

        [Route("{username}")]
        [HttpGet, Description("Get player")]
        public async Task<GetPlayerResponse> GetSingleAsync(string username)
        {
            var player = await _queries.GetSingleAsync(p => p.Username == username);

            if (player == null)
            {
                return CreateInvalidResponse<GetPlayerResponse>(Resources.Player.NotFound);
            }

            var playerModel = new PlayerModel
            {
                Username = player.Username,
                RealName = player.RealName,
                Phone = player.Phone,
                Email = player.Email,
                QQ = player.QQ,
                WeChat = player.Wechat
            };

            return new GetPlayerResponse
            {
                Valid = true,
                Player = playerModel
            };
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Route("forgotPassword")]
        [HttpPost(), Description("Forgot passowrd")]
        public Task ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            return Task.FromResult(0);
        }

        [Route("{username}/contact")]
        [HttpPut, Description("Update player contact")]
        public async Task<MessageResponse> UpdateAsync(string username, [FromBody] UpdatePlayerRequest request)
        {
            var player = await _queries.GetSingleAsync(p => p.Username == username);

            if (player == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.NotFound);
            }

            player.RealName = request.RealName;
            player.Phone = request.Phone;
            player.Email = request.Email;
            player.QQ = request.QQ;
            player.Wechat = request.Wechat;
            player.UpdatedByUsername = Username;
            player.UpdatedOn = DateTimeOffset.Now;
            await _commands.UpdateAsync(player);
            await CreateAuditLogAsync("Update Player Contact", player.PlayerCode);
            return CreateValidResponse<MessageResponse>();
        }

        [Route("{username}/password")]
        [HttpPut, Description("Update player password")]
        public async Task<MessageResponse> UpdatePasswordAsync(string username, [FromBody] UpdatePasswordRequest request)
        {
            var player = await _queries.GetSingleAsync(p => p.Username == username);

            if (player == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.NotFound);
            }

            if (_hashProvider.PasswordMismatch(player.Password, player.Salt, request.OldPassword))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.PasswordMismatched);
            }

            var (salt, pwd) = _hashProvider.GenerateCredential(request.NewPassword);
            player.Password = pwd;
            player.Salt = salt;
            player.UpdatedByUsername = Username;
            player.UpdatedOn = DateTimeOffset.Now;
            await _commands.UpdateAsync(player);
            await CreateAuditLogAsync("Update player password", player.Username);
            return CreateValidResponse<MessageResponse>();
        }

        private async Task CreateAuditLogAsync(string operation, string username)
        {
            var auditLog = new PlayerAuditLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ModuleName = "Player",
                CreatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                Operation = operation,
                Username = username
            };

            await _auditLogCommands.CreateAsync(auditLog);
        }

        private Guid GetLicenseeIdFromHttpContext()
        {
            return HttpContext.Items.TryGetValue(Consts.ContextItems.LicenseeId, out var v) ? (Guid)v : Guid.Empty;
        }

    }
}
