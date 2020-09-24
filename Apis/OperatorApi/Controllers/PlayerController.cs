using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Controllers.Messages.Players;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class PlayerController : BaseContextController
    {
        private readonly ITgsQueries<Player> _queries;
        private readonly ITgsCommands<Player> _commands;
        private readonly ITgsCommands<PlayerLoginRecord> _playerLoginRecordCommands;
        private readonly ITgsCommands<AuditLog> _auditLogCommands;
        private readonly IHashProvider _hashProvider;
        private readonly ITgsHashConfig _tgsHashConfig;
        private readonly ICaptchaFactory _captchaFactory;

        public PlayerController(
            IHashProvider hashProvider,
            ITgsHashConfig tgsHashConfig,
            ITgsQueries<Player> queries,
            ITgsCommands<Player> commands,
            ITgsCommands<AuditLog> auditLogCommands,
            ITgsCommands<PlayerLoginRecord> playerLoginRecordCommands,
            ICaptchaFactory captchaFactory,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _hashProvider = hashProvider;
            _tgsHashConfig = tgsHashConfig;
            _queries = queries;
            _commands = commands;
            _auditLogCommands = auditLogCommands;
            _playerLoginRecordCommands = playerLoginRecordCommands;
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
        public async Task<MessageResponse> CreateAsync([FromBody] CreatePlayerRequest request)
        {
            //var verifiedResult = await _captchaFactory.VerifyAsync(request.Answer, request.Captcha);
            //if (verifiedResult.Code != 100)
            //{
            //    return CreateInvalidResponse<MessageResponse>(verifiedResult.Message);
            //}

            var exists = await _queries.ExistsAsync(p => p.Username == request.Username);
            if (exists)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.UsernameExists);
            }

            var licenseeId = _httpContextAccessor.HttpContext.Request.Headers["LicenseeId"];

            var (salt, password) = GenerateCredential(request.Password);
            var defaultCulture = "zh-CN";
            var defaultCurrency = "RMB";

            var playerCode = PlayerCodeGenerator.NewId();

            var wallet = new Wallet
            {
                Id = GuidExtensions.NewSequentialGuid(),
                PlayerCode = playerCode,
                CurrencyCode = defaultCurrency,
                ExternalCurrencyCode = defaultCurrency,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            var player = new Player
            {
                PlayerCode = playerCode,
                Username = request.Username,
                Password = password,
                Salt = salt,
                InvitationCode = CodeGenerator.NewId(6),
                LicenseeId = new Guid(licenseeId),
                CultureCode = defaultCulture,
                DefaultWalletCurrencyCode = defaultCurrency,
                RegisteredOn = DateTimeOffset.Now,
                LastActiveOn = DateTimeOffset.Now,
                Status = PlayerStatus.Enabled,
                Type = PlayerTypes.RealPlayer,
                BetLimitGroupId = 1,
                UpdatedByUsername = request.Username,
                UpdatedOn = DateTimeOffset.Now,
                Wallets = new HashSet<Wallet> {wallet}
            };

            await _commands.CreateAsync(player);
            await CreateAuditLogAsync(request.Username, player.PlayerCode);
            return CreateValidResponse<MessageResponse>();
        }

        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost("login"), Description("login")]
        public async Task<MessageResponse> LoginAsync([FromBody] LoginRequest request)
        {
            var player = await _queries.GetSingleAsync(p => p.Username == request.Username);

            if (player == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.NotFound);
            }

            if (PasswordMismatch(player, request.Password))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.PasswordMismatched);
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
                PlatformType = PlatformType.Desktop
            };

            await SignInAsync(player);
            await _playerLoginRecordCommands.CreateAsync(logRecord);
            return CreateValidResponse<MessageResponse>();
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

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

            if (PasswordMismatch(player, request.OldPassword))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Player.PasswordMismatched);
            }

            var (salt, pwd) = GenerateCredential(request.NewPassword);
            player.Password = pwd;
            player.Salt = salt;
            player.UpdatedByUsername = Username;
            player.UpdatedOn = DateTimeOffset.Now;
            await _commands.UpdateAsync(player);
            await CreateAuditLogAsync("Update player password", player.PlayerCode);
            return CreateValidResponse<MessageResponse>();
        }

        private async Task CreateAuditLogAsync(string username, string playerCode)
        {
            var auditLog = new AuditLog
            {
                Id = GuidExtensions.NewSequentialGuid(),
                ModuleName = "Player",
                CreatedByUsername = Username,
                CreatedOn = DateTimeOffset.Now,
                Operation = $"Create Player '{username}'",
                PlayerCode = playerCode
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

        private bool PasswordMismatch(Player player, string password)
        {
            var salt = _hashProvider.Hash(player.Salt);

            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                salt,
                _tgsHashConfig.HashAlgorithm);

            return !player.Password.SequenceEqual(hashedPassword);
        }
    }
}
