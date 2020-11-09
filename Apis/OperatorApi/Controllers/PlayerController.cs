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
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;
using TokenExpireSeconds = TGS.Core.Models.Consts.TokenExpireSeconds;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class PlayerController : BaseContextController
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IStatusValidator _statusValidator;
        private readonly IPlayerQueries _playerQueries;
        private readonly ICaptchaFactory _captchaFactory;

        public PlayerController(
            ITokenProvider tokenProvider,
            IPasswordHandler passwordHandler,
            IStatusValidator statusValidator,
            IPlayerQueries playerQueries,
            IPlayerOperations playerOperations,
            IRequestAnalyzer requestAnalyzer,
            ICaptchaFactory captchaFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor)
        {
            _tokenProvider = tokenProvider;
            _passwordHandler = passwordHandler;
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
        public CaptchaInfo GetCaptcha()
        {
            return _captchaFactory.Create();
        }

        /// <summary>
        /// Check username exists
        /// </summary>
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost, Route("checkUsername")]
        public async Task<CheckPlayerResponse> UsernameExistsAsync(CheckPlayerRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
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
        [HttpPost, Route("create")]
        public async Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request)
        {
            // TODO: Verify captcha via middleware
           
            var licenseeId = HttpContext.Items.GetLicenseeId();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNotNullOrEmpty())
            {
                return new CreatePlayerResponse { IsNew = false };
            }

            var (salt, password) = _passwordHandler.GenerateCredential(request.Password);

            // TODO: get parent player code via parent invitation code

            await _playerOperations.CreatePlayerAsync(new CreatePlayerData
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
                Type = request.IsTestPlayer ? PlayerType.OperatorTestPlayer : PlayerType.RealPlayer,
                HasAgent = request.HasAgent
            });

            return new CreatePlayerResponse
            {
                IsNew = true
            };
        }

        /// <summary>
        /// Player login
        /// </summary>
        [AllowAnonymous]
        [HttpPost, Route("login")]
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var username = request.Username;
            var licenseeId = HttpContext.Items.GetLicenseeId();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.Username);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(username, Resources.Player.NotFound);
            }

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_passwordHandler.Verify(password, salt, request.Password))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            _statusValidator.ThrowIfPlayerDisabled(PlayerStatus.Enabled);

            var createdOn = DateTimeOffset.UtcNow;
            var ipAddress = _requestAnalyzer.GetIp(HttpContext);

            var record = await _playerOperations.PlayerLoginAsync(new PlayerLoginData
            {
                LicenseeId = licenseeId,
                Username = username,
                PlayerCode = playerCode,
                PlatformType = request.PlatformType,
                IpAddress = ipAddress,
                CreatedOn = createdOn,
                ExpiredOn = createdOn.AddSeconds(TokenExpireSeconds.PlayerLogin)
            });

            await SignInAsync(username);

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode,
                LoginRecordId = record.Id,
                PlatformType = record.PlatformType,
                ExpiredOn = record.ExpiredOn
            };

            return new LoginResponse
            {
                PlayerToken = _tokenProvider.Encrypt(playerTokenData),
                LoginTime = createdOn
            };
        }

        /// <summary>
        /// Player logout, player login is needed when use this
        /// </summary>
        [Authorize]
        [HttpPost, Route("logout")]
        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            var username = request.PlayerToken;
            var licenseeId = HttpContext.Items.GetLicenseeId();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.PlayerToken);
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

            await HttpContext.SignOutAsync();

            return new LogoutResponse { UpdateTime = updatedOn };
        }

        /// <summary>
        /// Get player information
        /// </summary>
        [AllowAnonymous]
        [HttpGet, Route("{username}")]
        public async Task<GetPlayerResponse> GetPlayerAsync(string username)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
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
        /// TokenExpireSeconds forget password
        /// </summary>
        [AllowAnonymous]
        [HttpPost, Route("forgotPassword"), Description("Forgot passowrd")]
        public Task ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Update player information, player login is needed when use this
        /// </summary>
        [Authorize(CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPut, Route("update")]
        public async Task<UpdatePlayerResponse> UpdatePlayerAsync(UpdatePlayerRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.PlayerToken);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(request.PlayerToken, Resources.Player.NotFound);
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
        /// TokenExpireSeconds change password, player login is needed when use this
        /// </summary>
        [Authorize(CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPut, Route("changePassword")]
        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, request.PlayerToken);
            if (playerCode.IsNullOrEmpty())
            {
                throw new PlayerNotFoundException(request.PlayerToken, Resources.Player.NotFound);
            }

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_passwordHandler.Verify(password, salt, request.OldPassword))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            var updatedOn = DateTimeOffset.UtcNow;
            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(request.NewPassword);
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

        private Task SignInAsync(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(Consts.UserInfo.UsernameKey, username),
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(TokenExpireSeconds.PlayerLogin),
                IsPersistent = true,
            };

            const string scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            var claimsIdentity = new ClaimsIdentity(claims, scheme);

            return HttpContext.SignInAsync(scheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}