using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
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
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;
using PlayerConsts = TGS.Core.Models.Consts.Player;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public partial class PlayerController : BaseContextController
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly IHashProvider _hashProvider;
        private readonly IStatusValidator _statusValidator;
        private readonly IPlayerQueries _playerQueries;
        private readonly ICaptchaFactory _captchaFactory;

        public PlayerController(
            IHashProvider hashProvider,
            IStatusValidator statusValidator,
            IPlayerQueries playerQueries,
            IPlayerOperations playerOperations,
            IRequestAnalyzer requestAnalyzer,
            ICaptchaFactory captchaFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor)
        {
            _hashProvider = hashProvider;
            _statusValidator = statusValidator;
            _playerQueries = playerQueries;
            _playerOperations = playerOperations;
            _requestAnalyzer = requestAnalyzer;
            _captchaFactory = captchaFactory;
        }

        /// <summary>
        /// Get captcha image, answer related information
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpGet, Route("captcha")]
        public async Task<CaptchaInfo> GetCaptcha()
        {
            var model = await _captchaFactory.CreateAsync();
            return model;
        }

        /// <summary>
        /// Check username exists
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
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

        /// <summary>
        /// Create a player
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost, Route("create"), Description("Create a player")]
        public async Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request)
        {
            //var result = await _captchaFactory.VerifyAsync(request.Answer, request.Captcha);
            //if (!result.IsValid)
            //{
            //    throw new InvalidOperationException(result.Message);
            //}

            var licenseeId = GetLicenseeIdFromHttpContext();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

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

        /// <summary>
        /// Player login
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost("login"), Description("login")]
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var username = request.Username;
            var licenseeId = GetLicenseeIdFromHttpContext();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(username, Resources.Player.NotFound);
            }

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_hashProvider.PasswordMatched(password, salt, request.Password))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);

            var createdOn = DateTimeOffset.UtcNow;
            var ipAddress = _requestAnalyzer.GetIp(HttpContext);

            await _playerOperations.PlayerLoginAsync(new PlayerLoginData
            {
                LicenseeId = licenseeId,
                Username = username,
                PlayerCode = playerCode,
                PlatformType = request.PlatformType,
                IpAddress = ipAddress,
                CreatedOn = createdOn,
                ExpiredOn = createdOn.AddSeconds(PlayerConsts.LoginExpirationSeconds)
            });

            await SignInAsync(username);
            return new LoginResponse { LoginTime = createdOn };
        }

        /// <summary>
        /// Player logout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost, Route("logout")]
        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            var username = request.Username;
            var licenseeId = GetLicenseeIdFromHttpContext();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(username, Resources.Player.NotFound);
            }

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations.PlayerLogoutAsync(new PlayerLogoutData
            {
                Username = username,
                PlayerCode = playerCode,
                PlatformType = request.PlatformType,
                UpdatedOn = updatedOn,
            });

            await HttpContext?.SignOutAsync();

            return new LogoutResponse { UpdateTime = updatedOn };
        }

        /// <summary>
        /// Get player information
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Route("{username}")]
        [HttpGet, Description("Get player")]
        public async Task<GetPlayerResponse> GetPlayerAsync(string username)
        {
            var licenseeId = GetLicenseeIdFromHttpContext();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var player = await _playerOperations.GetPlayerByUsernameAsync(licenseeId, username);
            if (player == null)
            {
                throw new PlayerNotFoundException(username, Resources.Player.NotFound);
            }

            return new GetPlayerResponse
            {
                Data = new()
                {
                    Username = player.Username,
                    RealName = player.RealName,
                    Email = player.Email,
                    Phone = player.Phone,
                    QQ = player.QQ,
                    Wechat = player.Wechat
                }
            };
        }

        /// <summary>
        /// Player forget password
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Route("forgotPassword")]
        [HttpPost(), Description("Forgot passowrd")]
        public Task ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Update player information
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [Route("update")]
        [HttpPut, Description("Update player information")]
        public async Task<UpdatePlayerResponse> UpdatePlayerAsync(UpdatePlayerRequest request)
        {
            var licenseeId = GetLicenseeIdFromHttpContext();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(request.Username, Resources.Player.NotFound);
            }

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations.UpdatePlayerAsync(new UpdatePlayerData
            {
                Username = Username,
                PlayerCode = playerCode,
                RealName = request.RealName,
                Phone = request.Phone,
                Email = request.Email,
                QQ = request.QQ,
                Wechat = request.Wechat,
                UpdatedOn = updatedOn
            });

            return new UpdatePlayerResponse
            {
                PlayerCode = playerCode,
                UpdateTime = updatedOn
            };
        }

        /// <summary>
        /// Player change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [Route("changePassword")]
        [HttpPut, Description("Change player password")]
        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var licenseeId = GetLicenseeIdFromHttpContext();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(request.Username, Resources.Player.NotFound);
            }

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_hashProvider.PasswordMatched(password, salt, request.OldPassword))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            var updatedOn = DateTimeOffset.UtcNow;
            var (newSalt, newPassword) = _hashProvider.GenerateCredential(request.NewPassword);
            await _playerOperations.ChangePlayerPasswordAsync(new ChangePlayerPasswordData
            {
                Username = Username,
                PlayerCode = playerCode,
                Salt = newSalt,
                Password = newPassword,
                UpdatedOn = updatedOn
            });

            return new ChangePasswordResponse
            {
                PlayerCode = playerCode,
                UpdateTime = updatedOn
            };
        }

        private Guid GetLicenseeIdFromHttpContext()
        {
            return HttpContext.Items.TryGetValue(Consts.ContextItems.LicenseeId, out var v) ? (Guid)v : Guid.Empty;
        }

        private async Task SignInAsync(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(Consts.UserInfo.UsernameKey, username),
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(PlayerConsts.LoginExpirationSeconds),
                IsPersistent = true,
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}