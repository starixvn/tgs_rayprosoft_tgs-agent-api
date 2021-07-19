using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.GameBalance;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/game/balance")]
    [ApiController]
    public class GameBalanceController : ControllerBase
    {
        /// <summary>
        /// Get transfer wallet balance
        /// </summary>
        /// <param name="getGameBalanceService"></param>
        /// <param name="gameStatusValidator"></param>
        /// <param name="gpCode"></param>
        /// <returns></returns>
        [HttpGet("{gpCode}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetGameBalanceResponse> GetGameBalanceAsync(
            [FromServices] IGetService<GetGameBalanceQuery, GameBalanceInfo> getGameBalanceService,
            [FromServices] IGameProviderStatusValidator gameStatusValidator,
            string gpCode)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            await gameStatusValidator.CheckGameProviderStatusAsync(playerTokenData, gpCode, brandId);

            var query = new GetGameBalanceQuery(playerTokenData.PlayerCode, gpCode);

            var info = await getGameBalanceService.GetAsync(query).ConfigureAwait(false);

            return new GetGameBalanceResponse
            {
                Balance = info.Balance,
                Error = info.Error,
                ErrorMessage = info.ErrorMessage
            };
        }

        /// <summary>
        /// Transfer wallet fund in(GP -> Tgs)
        /// </summary>
        /// <param name="submitGameBalanceFundInCommandService"></param>
        /// <param name="gameStatusValidator"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("fund/in")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask SubmitGameBalanceFundInRequestAsync(
            [FromServices] ICommandService<SubmitGameBalanceFundInCommand> submitGameBalanceFundInCommandService,
            [FromServices] IGameProviderStatusValidator gameStatusValidator,
            [FromBody] SubmitGameBalanceFundInRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            await gameStatusValidator.CheckGameProviderStatusAsync(playerTokenData, request.GameProviderCode, brandId);

            var command = new SubmitGameBalanceFundInCommand
            (
                request.GameProviderCode,
                playerTokenData.PlayerCode,
                request.Amount
            );

            await submitGameBalanceFundInCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Transfer wallet fund out(Tgs -> Gp)
        /// </summary>
        /// <param name="submitGameBalanceFundOutCommandService"></param>
        /// <param name="gameStatusValidator"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("fund/out")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask SubmitGameBalanceFundOutRequestAsync(
            [FromServices] ICommandService<SubmitGameBalanceFundOutCommand> submitGameBalanceFundOutCommandService,
            [FromServices] IGameProviderStatusValidator gameStatusValidator,
            [FromBody] SubmitGameBalanceFundOutRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            await gameStatusValidator.CheckGameProviderStatusAsync(playerTokenData, request.GameProviderCode, brandId);

            var command = new SubmitGameBalanceFundOutCommand
            (
                request.GameProviderCode,
                playerTokenData.PlayerCode,
                request.Amount
            );

            await submitGameBalanceFundOutCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }
    }
}
