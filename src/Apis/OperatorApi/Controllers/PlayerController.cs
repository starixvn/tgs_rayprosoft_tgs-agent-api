using System;
using System.Globalization;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.IdentityVerifiers;
using TGS.Services.Operations;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;

using CoreExceptions = TGS.Core.Models.Exceptions;
using PlayerNotFoundException = TGS.Services.Operator.PlayerNotFoundException;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public partial class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(ILogger<PlayerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Player changes password, player login is needed when use this
        /// </summary>
        [HttpPut("password")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ChangePasswordResponse> ChangePasswordAsync(
            [FromServices] IGetService<ChangePasswordQuery, ChangePasswordInfo> getChangePasswordQueryService,
            [FromBody] ChangePasswordRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new ChangePasswordQuery(
                brandId,
                playerTokenData.PlayerCode,
                request.OldPassword,
                request.NewPassword);

            ChangePasswordInfo info;
            try
            {
                info = await getChangePasswordQueryService
                    .GetAsync(query)
                    .ConfigureAwait(false);
            }
            catch (Core.Models.Exceptions.InvalidCredentialsException)
            {
                throw new Core.Models.Exceptions.InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            return new ChangePasswordResponse()
            {
                PlayerCode = info.PlayerCode,
                UpdateTime = info.UpdateTime
            };
        }

        /// <summary>
        /// Checks the identity is valid.
        /// </summary>
        /// <param name="identityVerifierSelector">The identity verifier selector.</param>
        /// <param name="playerOperations">The player operations.</param>
        /// <param name="id">The identify number.</param>
        /// <returns></returns>
        [HttpGet("identity/check/{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<bool> CheckIdentityIsValidAsync(
            [FromServices] IIdentityVerifierSelector<string> identityVerifierSelector,
            [FromServices] IPlayerOperations playerOperations,
            string id)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var player = await playerOperations
                .GetPlayerByCodeAsync(playerTokenData.PlayerCode)
                .ConfigureAwait(false);

            var identityVerifier = identityVerifierSelector.GetIdentityVerifier(player.DefaultWalletCurrencyCode);

            return identityVerifier.Verify(id);
        }

        /// <summary>
        /// Check username exists
        /// </summary>
        [HttpGet("checkUsername")]
        public async Task<CheckUsernameResponse> CheckUsernameAsync(
            [FromServices] IGetService<GetUserNameQuery, bool> getUserNameQueryService,
            [FromQuery] CheckUsernameRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new GetUserNameQuery(brandId, request.Username);
            var isExist = await getUserNameQueryService.GetAsync(query).ConfigureAwait(false);

            return new CheckUsernameResponse()
            {
                Exists = isExist
            };
        }

        /// <summary>
        /// Create a player
        /// </summary>
        [HttpPost("create")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<CreatePlayerResponse> CreateAsync(
            [FromServices] IRequestAnalyzer requestAnalyzer,
            [FromServices] IGetService<CreatePlayerCommand, CreatePlayerInfo> createPlayerQueryService,
            [FromBody] CreatePlayerRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var ipAddress = requestAnalyzer.GetIp(HttpContext);
            var query = new CreatePlayerCommand(
                brandId,
                request.Username,
                request.Password,
                request.CurrencyCode,
                request.AgentInvitationCode,
                request.IsTestPlayer,
                ipAddress);
            var info = await createPlayerQueryService.GetAsync(query).ConfigureAwait(false);

            return new CreatePlayerResponse()
            {
                PlayerCode = info.PlayerCode,
                IsNew = info.IsNew
            };
        }

        /// <summary>
        /// Start forgot password flow.
        /// </summary>
        /// <param name="forgotPasswordService">The forgot password service.</param>
        /// <param name="request">The request.</param>
        /// <response code="200">Return the token, when reset password, must input to verify.</response>
        [HttpPost("password/forgot")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ValueTask<string> ForgotPasswordAsync(
            [FromServices] IGetService<ForgotPasswordCommand, string> forgotPasswordService,
            [FromBody] ForgotPasswordRequest request)
            => forgotPasswordService.GetAsync(new ForgotPasswordCommand(
                CultureInfo.CurrentUICulture,
                request.Email));

        [HttpGet("game/auto-transfer")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask<bool> GetAutoTransferToGameFlagAsync(
                    [FromServices] IGetService<GetPlayerIsAutoTransferToGameQuery, bool> getService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetPlayerIsAutoTransferToGameQuery(playerTokenData.PlayerCode);

            return getService.GetAsync(query);
        }

        /// <summary>
        /// Get player information
        /// </summary>
        [HttpGet("info")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetPlayerResponse> GetPlayerAsync(
            [FromServices] IGetService<GetPlayerQuery, PlayerDataInfo> getPlayerQueryService,
            [FromServices] IGetService<GetPlayerIsAutoTransferToGameQuery, bool> getAutoTransFlagService)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var playerCode = playerTokenData.PlayerCode;

            var query = new GetPlayerQuery(brandId, playerCode);

            var playerData = new PlayerData();
            try
            {
                var player = await getPlayerQueryService.GetAsync(query).ConfigureAwait(false);

                playerData.Username = player.Username;
                playerData.RealName = player.RealName;
                playerData.Email = player.Email;
                playerData.Phone = player.Phone;
                playerData.QQ = player.QQ;
                playerData.Wechat = player.Wechat;
                playerData.IsSetPayPassword = player.IsSetPayPassword;
                playerData.IdentityAuthentication = player.IdentityAuthentication;
                playerData.IsAutoTransferToGame = await getAutoTransFlagService
                    .GetAsync(new GetPlayerIsAutoTransferToGameQuery(playerCode))
                    .ConfigureAwait(false);
            }
            catch (PlayerNotFoundException)
            {
                throw new CoreExceptions.PlayerNotFoundException(query.PlayerCode, Resources.Player.NotFound);
            }

            return new GetPlayerResponse()
            {
                Data = playerData
            };
        }

        /// <summary>
        /// Player login
        /// </summary>
        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LoginResponse> LoginAsync(
            [FromServices] IRequestAnalyzer requestAnalyzer,
            [FromServices] ICommandService<CheckPlayerBannedCommand> commandService,
            [FromServices] IGetService<LoginCommand, LoginInfo> loginService,
            [FromBody] LoginRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            try
            {
                var ipAddress = requestAnalyzer.GetIp(HttpContext);

                await commandService
                    .ExecuteAsync(new CheckPlayerBannedCommand(brandId, request.Username))
                    .ConfigureAwait(false);

                var query = new LoginCommand(
                    brandId,
                    request.Username,
                    request.Password,
                    ipAddress,
                    request.PlatformType, Consts.Site);

                var info = await loginService
                    .GetAsync(query)
                    .ConfigureAwait(false);

                return new LoginResponse()
                {
                    LoginTime = info.LoginTime,
                    PlayerToken = info.PlayerToken
                };
            }
            catch (PlayerNotFoundException)
            {
                throw new CoreExceptions.PlayerNotFoundException(request.Username, Resources.Player.NotFound);
            }
            catch (CoreExceptions.InvalidCredentialsException)
            {
                throw new CoreExceptions.InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case CoreExceptions.PlayerNotFoundException playerNotFoundException:
                        _logger.LogWarning(playerNotFoundException, $"{request.Username} not found.");
                        break;

                    case CoreExceptions.InvalidCredentialsException invalidCredException:
                        _logger.LogWarning(invalidCredException, Resources.Player.PasswordMismatched);
                        break;

                    default:
                        _logger.LogWarning(ex, "Login fail!");
                        break;
                }

                throw new LoginFailException();
            }
        }

        /// <summary>
        /// Player logout, player login is needed when use this
        /// </summary>
        [HttpDelete("logout")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<LogoutResponse> LogoutAsync(
            [FromServices] IGetService<GetLogoutQuery, LogoutInfo> getLogoutQueryService)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetLogoutQuery(brandId, playerTokenData);
            LogoutInfo info;
            try
            {
                info = await getLogoutQueryService
                    .GetAsync(query)
                    .ConfigureAwait(false);
            }
            catch (CoreExceptions.PlayerNotFoundException)
            {
                throw new CoreExceptions.PlayerNotFoundException(playerTokenData.PlayerCode, Resources.Player.NotFound);
            }

            return new LogoutResponse()
            {
                UpdateTime = info.UpdateTime
            };
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="resetService">The reset service.</param>
        /// <param name="request">The request.</param>
        /// <response code="406">Reset password not accept, maybe validate code not match.</response>
        [HttpPost("password/reset")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async ValueTask ResetPasswordAsync(
            [FromServices] ICommandService<ResetPasswordCommand> resetService,
            [FromBody] ResetPasswordRequest request)
        {
            await resetService
                .ExecuteAsync(new ResetPasswordCommand(request.SecurityKey, request.ValidateCode, request.NewPassword))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the player identity.
        /// </summary>
        /// <param name="setIdentityService">The set identity service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPatch("identity")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask SetPlayerIdentityAsync(
            [FromServices] ICommandService<SetIdentityCommand> setIdentityService,
            [FromBody] SetPlayerIdentityRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetIdentityCommand(playerTokenData.PlayerCode, request.RealName, request.Identity);

            return setIdentityService.ExecuteAsync(command);
        }

        /// <summary>
        /// Sets the qq number.
        /// </summary>
        /// <param name="setQqService">The set qq service.</param>
        /// <param name="request">The request.</param>
        [HttpPatch("qq")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task SetQqIdAsync(
            [FromServices] ICommandService<SetQqCommand> setQqService,
            [FromBody] SetQqIdRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetQqCommand(playerTokenData.PlayerCode, request.QqId);

            await setQqService.ExecuteAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the we chat identifier.
        /// </summary>
        /// <param name="setWeChatService">The set we chat service.</param>
        /// <param name="request">The request.</param>
        [HttpPatch("wechat")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task SetWeChatIdAsync(
            [FromServices] ICommandService<SetWeChatCommand> setWeChatService,
            [FromBody] SetWeChatIdRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetWeChatCommand(playerTokenData.PlayerCode, request.WebChatId);

            await setWeChatService.ExecuteAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the change email.
        /// </summary>
        /// <param name="setPlayerEmailService">The set player email service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("email")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask<string> StartChangeEmailAsync(
            [FromServices] IGetService<StartSetPlayerEmailCommand, string> setPlayerEmailService,
            [FromBody] StartSetEmailRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new StartSetPlayerEmailCommand(playerTokenData.PlayerCode, request.Email, CultureInfo.CurrentUICulture);

            return setPlayerEmailService.GetAsync(command);
        }

        /// <summary>
        /// Starts the change phone.
        /// </summary>
        /// <param name="startSetPlayerPhoneService">The start set player phone service.</param>
        /// <param name="request">The request.</param>
        [HttpPost("phone")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask<string> StartChangePhoneAsync(
            [FromServices] IGetService<StartSetPlayerPhoneCommand, string> startSetPlayerPhoneService,
            [FromBody] StartSetPhoneRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new StartSetPlayerPhoneCommand(
                playerTokenData.PlayerCode,
                request.AreaCode,
                request.Phone,
                CultureInfo.CurrentUICulture);

            return startSetPlayerPhoneService.GetAsync(command);
        }

        /// <summary>
        /// Submits the change email.
        /// </summary>
        /// <param name="commitPlayerEmailService">The commit player email service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut("email")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask SubmitChangeEmailAsync(
            [FromServices] ICommandService<CommitSetPlayerEmailCommand> commitPlayerEmailService,
            [FromBody] SubmitChangeEmailRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var command = new CommitSetPlayerEmailCommand(
                playerTokenData.PlayerCode,
                request.SecurityKey,
                request.ValidateCode,
                brandId);

            return commitPlayerEmailService.ExecuteAsync(command);
        }

        /// <summary>
        /// Submits the change phone.
        /// </summary>
        /// <param name="commitPlayerPhoneService">The commit player phone service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPut("phone")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask SubmitChangePhoneAsync(
            [FromServices] ICommandService<CommitSetPlayerPhoneCommand> commitPlayerPhoneService,
            [FromBody] SubmitChangePhoneRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new CommitSetPlayerPhoneCommand(
                playerTokenData.PlayerCode,
                request.SecurityKey,
                request.ValidateCode);

            return commitPlayerPhoneService.ExecuteAsync(command);
        }

        [HttpPost("game/auto-transfer")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask ToggleAutoTransferToGameFlagAsync(
            [FromServices] ICommandService<TogglePlayerIsAutoTransferToGameCommand> toggleService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new TogglePlayerIsAutoTransferToGameCommand(playerTokenData.PlayerCode);

            return toggleService.ExecuteAsync(command);
        }
    }
}
