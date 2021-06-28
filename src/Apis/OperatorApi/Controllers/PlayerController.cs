using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;
using CoreExceptions = TGS.Core.Models.Exceptions;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public partial class PlayerController : ControllerBase
    {
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
            [FromServices] IGetService<GetCreatePlayerQuery, CreatePlayerInfo> getCreatePlayerQueryService,
            [FromBody] CreatePlayerRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new GetCreatePlayerQuery(
                brandId,
                request.Username,
                request.Password,
                request.CurrencyCode,
                request.AgentInvitationCode,
                request.IsTestPlayer);
            var info = await getCreatePlayerQueryService.GetAsync(query).ConfigureAwait(false);

            return new CreatePlayerResponse()
            {
                PlayerCode = info.PlayerCode,
                IsNew = info.IsNew
            };
        }

        /// <summary>
        /// Player login
        /// </summary>
        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LoginResponse> LoginAsync(
            [FromServices] IRequestAnalyzer requestAnalyzer,
            [FromServices] IGetService<GetLoginQuery, LoginInfo> getLoginQueryService,
            [FromBody] LoginRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var ipAddress = requestAnalyzer.GetIp(HttpContext);

            var query = new GetLoginQuery(
                brandId,
                request.Username,
                request.Password,
                ipAddress,
                request.PlatformType, Consts.Site);

            LoginInfo info;
            try
            {
                info = await getLoginQueryService.GetAsync(query).ConfigureAwait(false);
            }
            catch (CoreExceptions.PlayerNotFoundException)
            {
                throw new CoreExceptions.PlayerNotFoundException(request.Username, Resources.Player.NotFound);
            }
            catch (CoreExceptions.InvalidCredentialsException)
            {
                throw new CoreExceptions.InvalidCredentialsException(Resources.Player.PasswordMismatched);
            }

            return new LoginResponse()
            {
                LoginTime = info.LoginTime,
                PlayerToken = info.PlayerToken
            };
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
        /// Get player information
        /// </summary>
        [HttpGet("info")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetPlayerResponse> GetPlayerAsync(
            [FromServices] IGetService<GetPlayerQuery, PlayerDataInfo> getPlayerQueryService)
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
        /// Sets the qq number.
        /// </summary>
        /// <param name="setQqService">The set qq service.</param>
        /// <param name="request">The request.</param>
        [HttpPatch("qq")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
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
        [TgsResponseWrapper]
        public async Task SetWeChatIdAsync(
            [FromServices] ICommandService<SetWeChatCommand> setWeChatService,
            [FromBody] SetWeChatIdRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new SetWeChatCommand(playerTokenData.PlayerCode, request.WebChatId);

            await setWeChatService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}