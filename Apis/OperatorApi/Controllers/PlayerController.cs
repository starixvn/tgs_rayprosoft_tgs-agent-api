using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Controllers.Messages.Players;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayerController : BaseContextController
    {
        private readonly Func<ITgsRepository> _tgsRepoFactory;
        private readonly ITgsQueries<Player> _queries;
        private readonly ITgsCommands<Player> _commands;
        private readonly ITgsCommands<Wallet> _walletCommands;
        private readonly ITgsCommands<PlayerLoginRecord> _playerLoginRecordCommands;
        private readonly ITgsCommands<PlayerAuditLog> _auditLogCommands;
        private readonly IHashProvider _hashProvider;
        private readonly ICaptchaFactory _captchaFactory;
        private readonly ITransactionManager _transactionManager;

        public PlayerController(
            Func<ITgsRepository> tgsRepoFactory,
            IHashProvider hashProvider,
            ITgsQueries<Player> queries,
            ITgsCommands<Player> commands,
            ITgsCommands<Wallet> walletCommands,
            ITgsCommands<PlayerAuditLog> auditLogCommands,
            ITgsCommands<PlayerLoginRecord> playerLoginRecordCommands,
            ICaptchaFactory captchaFactory,
            ITransactionManager transactionManager,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _tgsRepoFactory = tgsRepoFactory;
            _hashProvider = hashProvider;
            _queries = queries;
            _commands = commands;
            _walletCommands = walletCommands;
            _auditLogCommands = auditLogCommands;
            _playerLoginRecordCommands = playerLoginRecordCommands;
            _transactionManager = transactionManager;
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
        [HttpGet, Route("/checkUsername"), Description("Check username exists")]
        public Task<bool> UsernameExistsAsync([FromQuery] string username)
        {
            return _queries.ExistsAsync(p => p.Username == username);
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost, Description("Create a player")]
        public async Task<TgsResponse> CreateAsync([FromBody] CreatePlayerRequest request)
        {
            var verifiedResult = await _captchaFactory.VerifyAsync(request.Answer, request.Captcha);
            if (verifiedResult.Code != 100)
            {
                throw new InvalidOperationException(verifiedResult.Message);
            }

            var licenseeId = new Guid(_httpContextAccessor.HttpContext.Request.Headers["LicenseeId"]);

            var exists = await _queries.ExistsAsync(p => p.Username == request.Username && p.LicenseeId == licenseeId);
            if (exists)
            {
                throw new InvalidOperationException(Resources.Player.UsernameExists);
            }

            var defaultCulture = "zh-CN";
            var defaultCurrency = "CNY";

            object p = await _transactionManager.ExecuteTransactionAsync(
                 _tgsRepoFactory,
                IsolationLevel.Serializable,
                request.Username,
                execute: async (r, trans) => {
                    await CreatePlayerAsync(request, defaultCulture, defaultCurrency, licenseeId);
                    await CreateAuditLogAsync($"Create Player '{request.Username}'", request.Username);
                    return Task.FromResult(0);
                });
            return new TgsResponse();
        }

        private async Task CreatePlayerAsync(
            CreatePlayerRequest request,  
            string defaultCulture, 
            string defaultCurrency, 
            Guid licenseeId)
        {
            var (salt, password) = _hashProvider.GenerateCredential(request.Password);
            var playerCode = PlayerCodeGenerator.NewId();    

            var wallet = new Wallet
            {
                Id = GuidExtensions.NewSequentialGuid(),
                PlayerCode = playerCode,
                CurrencyCode = defaultCurrency,
                ExternalCurrencyCode = defaultCurrency,
                Balance = 0,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            var player = new Player
            {
                PlayerCode = playerCode,
                Username = request.Username,
                Password = password,
                Salt = salt,
                InvitationCode = StringGenerator.GenerateAlphanumericString(6),
                LicenseeId = licenseeId,
                CultureCode = defaultCulture,
                DefaultWalletCurrencyCode = defaultCurrency,
                ParentInvitationCode = request.ParentInvitationCode,
                RegisteredOn = DateTimeOffset.Now,
                LastActiveOn = DateTimeOffset.Now,
                Status = PlayerStatus.Enabled,
                Type = PlayerTypes.RealPlayer,
                BetLimitGroupId = 1,
                UpdatedByUsername = request.Username,
                UpdatedOn = DateTimeOffset.Now,
                Wallets = new HashSet<Wallet> { wallet }
            };

            await _commands.CreateAsync(player);
            await _walletCommands.CreateAsync(wallet);
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost("login"), Description("login")]
        public async Task<TgsResponse> LoginAsync([FromBody] LoginRequest request)
        {
            var licenseeId = new Guid(_httpContextAccessor.HttpContext.Request.Headers["LicenseeId"]);
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
            if (_httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();

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

            await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [HttpPost, Route("logout")]
        public Task LogoutAsync()
        {
            return _httpContextAccessor.HttpContext?.SignOutAsync();
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
    }
}
