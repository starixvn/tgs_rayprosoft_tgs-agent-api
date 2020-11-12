using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Apis.OperatorApi.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.IO;
using TGS.Core.Infrastructures.Json;
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
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHashProvider _hashProvider;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IStatusValidator _statusValidator;
        private readonly IPlayerQueries _playerQueries;
        private readonly ICaptchaFactory _captchaFactory;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IJsonSerializationProvider _json;

        public PlayerController(
            IPlayerQueries playerQueries,
            IPlayerOperations playerOperations,
            ILicenseeOperations licenseeOperations,
            ICaptchaFactory captchaFactory,
            IJwtTokenHandler jwtTokenHandler,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IPasswordHandler passwordHandler,
            IJsonSerializationProvider json,
            IStatusValidator statusValidator,
            IRequestAnalyzer requestAnalyzer,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor)
        {
            _tokenProvider = tokenProvider;
            _hashProvider = hashProvider;
            _passwordHandler = passwordHandler;
            _statusValidator = statusValidator;
            _playerQueries = playerQueries;
            _playerOperations = playerOperations;
            _licenseeOperations = licenseeOperations;
            _requestAnalyzer = requestAnalyzer;
            _captchaFactory = captchaFactory;
            _jwtTokenHandler = jwtTokenHandler;
            _json = json;
        }

        /// <summary>
        /// Get captcha image, answer related information
        /// </summary>
        [AllowAnonymous]
        [HttpGet, Route("captcha")]
        public CaptchaInfo GetCaptcha()
        {
            return _captchaFactory.Create();
        }

        /// <summary>
        /// Check username exists
        /// </summary>
        [AllowAnonymous]
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
        [AllowAnonymous]
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

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode,
                LoginRecordId = record.Id,
                PlatformType = record.PlatformType,
                ExpiredOn = record.ExpiredOn
            };
            
            var token = await GeneratePlayerJwtToken(licenseeId, playerTokenData);

            return new LoginResponse
            {
                PlayerToken = token,
                LoginTime = createdOn
            };
        }

        /// <summary>
        /// Player logout, player login is needed when use this
        /// </summary>
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [HttpPost, Route("logout")]
        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);
            
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);
            if (cachedPlayer == null)
            {
                throw new PlayerNotFoundException(playerCode, Resources.Player.NotFound);
            }

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations.PlayerLogoutAsync(new PlayerLogoutData
            {
                Username = cachedPlayer.Username,
                PlayerCode = playerCode,
                LoginRecordId = playerTokenData.LoginRecordId,
                UpdatedOn = updatedOn,
            });

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
        /// Player forget password
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
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
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
        /// Player change password, player login is needed when use this
        /// </summary>
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
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

        private async Task<string> GeneratePlayerJwtToken(Guid licenseeId, PlayerTokenData playerTokenData)
        {
            var data = _tokenProvider.Encrypt(playerTokenData);

            var jti = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, data),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            });

            var payload = _hashProvider.Hash($"{data}:{jti}");
            var secretKey = await _licenseeOperations.GetLicenseeSecretKeyAsync(licenseeId);
            var signature = Encoding.UTF8.GetBytes($"{payload}:{secretKey}");

            return _jwtTokenHandler.GenerateToken(identity, signature, Consts.Site,
                playerTokenData.ExpiredOn.DateTime);
        }
    }
}