using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
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
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Models.Services;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class PlayerController : ControllerBase
    {
        private readonly ICaptchaFactory _captchaFactory;
        private readonly IBrandCurrencyCodeMapper _currencyCodeMapper;
        private readonly IHashProvider _hashProvider;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IBrandLanguageCodeMapper _languageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPlayerQueries _playerQueries;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly IStatusValidator _statusValidator;
        private readonly ITokenProvider _tokenProvider;

        public PlayerController(
            IPlayerQueries playerQueries,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IBrandCurrencyCodeMapper currencyCodeMapper,
            IBrandLanguageCodeMapper languageCodeMapper,
            ICaptchaFactory captchaFactory,
            IJwtTokenHandler jwtTokenHandler,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IPasswordHandler passwordHandler,
            IStatusValidator statusValidator,
            IRequestAnalyzer requestAnalyzer)
        {
            _tokenProvider = tokenProvider;
            _hashProvider = hashProvider;
            _passwordHandler = passwordHandler;
            _statusValidator = statusValidator;
            _playerQueries = playerQueries;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _currencyCodeMapper = currencyCodeMapper;
            _languageCodeMapper = languageCodeMapper;
            _requestAnalyzer = requestAnalyzer;
            _captchaFactory = captchaFactory;
            _jwtTokenHandler = jwtTokenHandler;
        }

        /// <summary>
        /// Get captcha image, answer related information
        /// </summary>
        [HttpGet]
        [Route("captcha")]
        public CaptchaInfo GetCaptcha() => _captchaFactory.Create();

        /// <summary>
        /// Check username exists
        /// </summary>
        [HttpPost]
        [Route("checkUsername")]
        public async Task<CheckPlayerResponse> UsernameExistsAsync(CheckPlayerRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, request.Username);

            return new CheckPlayerResponse
            {
                Exists = playerCode.IsNotNullOrEmpty()
            };
        }

        /// <summary>
        /// Create a player
        /// </summary>
        [HttpPost]
        [Route("create")]
        public async Task<CreatePlayerResponse> CreateAsync(CreatePlayerRequest request)
        {
            // TODO: Verify captcha via middleware

            var brandId = HttpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var username = request.Username;
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, username);
            if (playerCode.IsNotNullOrEmpty())
            {
                return new CreatePlayerResponse {IsNew = false};
            }

            var externalCurrency = request.CurrencyCode;
            var currency = await _currencyCodeMapper.ExternalToInternalAsync(externalCurrency, brandId);
            var language = await _languageCodeMapper.ExternalToInternalAsync(request.Language, brandId);

            var (salt, password) = _passwordHandler.GenerateCredential(request.Password);

            // TODO: get parent player code via parent invitation code

            await _playerOperations.CreatePlayerAsync(new CreatePlayerData
            {
                BrandId = brandId,
                Username = username,
                Salt = salt,
                Password = password,
                CultureCode = language,
                CurrencyCode = currency,
                ExternalCurrencyCode = externalCurrency,
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
        [HttpPost]
        [Route("login")]
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var username = request.Username;
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(brandId, username);
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
                BrandId = brandId,
                Username = username,
                PlayerCode = playerCode,
                PlatformType = request.PlatformType,
                IpAddress = ipAddress,
                CreatedOn = createdOn,
                ExpiredOn = createdOn.AddSeconds(Core.Models.Consts.TokenExpireSeconds.PlayerLogin)
            });

            var playerTokenData = new PlayerTokenData
            {
                PlayerCode = playerCode,
                LoginRecordId = record.Id,
                PlatformType = record.PlatformType,
                ExpiredOn = record.ExpiredOn
            };

            var token = await GeneratePlayerJwtToken(brandId, playerTokenData);

            return new LoginResponse
            {
                PlayerToken = token,
                LoginTime = createdOn
            };
        }

        /// <summary>
        /// Player logout, player login is needed when use this
        /// </summary>
        [HttpPost]
        [Route("logout")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<LogoutResponse> LogoutAsync()
        {
            var brandId = HttpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

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
                UpdatedOn = updatedOn
            });

            return new LogoutResponse
            {
                UpdateTime = updatedOn
            };
        }

        /// <summary>
        /// Get player information
        /// </summary>
        [HttpGet]
        [Route("info")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetPlayerResponse> GetPlayerAsync()
        {
            var brandId = HttpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var player = await _playerOperations.GetPlayerByCodeAsync(playerCode);
            if (player == null)
            {
                throw new PlayerNotFoundException(playerCode, Resources.Player.NotFound);
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
        [HttpPost]
        [Route("forgotPassword")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update player information, player login is needed when use this
        /// </summary>
        [HttpPut]
        [Route("update")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<UpdatePlayerResponse> UpdatePlayerAsync(UpdatePlayerRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);

            var updatedOn = DateTimeOffset.UtcNow;

            await _playerOperations.UpdatePlayerAsync(new UpdatePlayerData
            {
                Username = cachedPlayer.Username,
                PlayerCode = cachedPlayer.PlayerCode,
                RealName = request.RealName,
                Phone = request.Phone,
                Email = request.Email,
                QQ = request.QQ,
                Wechat = request.Wechat,
                UpdatedOn = updatedOn
            });

            return new UpdatePlayerResponse
            {
                UpdateTime = updatedOn
            };
        }

        /// <summary>
        /// Player change password, player login is needed when use this
        /// </summary>
        [HttpPut]
        [Route("changePassword")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var (salt, password) = await _playerQueries.GetSaltAndPasswordByPlayerCodeAsync(playerCode);
            if (!_passwordHandler.Verify(password, salt, request.OldPassword))
            {
                throw new InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            var username = await _playerOperations.GetUsernameByPlayerCodeAsync(playerCode);
            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(request.NewPassword);
            await _playerOperations.ChangePlayerPasswordAsync(new ChangePlayerPasswordData
            {
                Username = username,
                PlayerCode = playerCode,
                Salt = newSalt,
                Password = newPassword,
                UpdatedOn = updatedOn
            });

            return new ChangePasswordResponse
            {
                UpdateTime = updatedOn
            };
        }

        private async Task<string> GeneratePlayerJwtToken(Guid brandId, PlayerTokenData playerTokenData)
        {
            var data = _tokenProvider.Encrypt(playerTokenData);

            var jti = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, data),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            });

            var payload = _hashProvider.Hash($"{data}:{jti}");
            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = Encoding.UTF8.GetBytes($"{payload}:{secretKey}");

            return _jwtTokenHandler.GenerateToken(identity, signature, Consts.Site,
                playerTokenData.ExpiredOn.DateTime);
        }
    }
}