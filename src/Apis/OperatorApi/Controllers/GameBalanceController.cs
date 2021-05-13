using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.GameBalance;
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
        /// <param name="gpCode"></param>
        /// <returns></returns>
        [HttpGet("{gpCode}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<GetGameBalanceResponse> GetGameBalanceAsync(
            [FromServices] IGetService<GetGameBalanceQuery, GameBalanceInfo> getGameBalanceService,
            string gpCode)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetGameBalanceQuery(playerTokenData.PlayerCode, gpCode);

            var info = await getGameBalanceService.GetAsync(query).ConfigureAwait(false);

            return new GetGameBalanceResponse
            {
                Balance = info.Balance
            };
        }

        /// <summary>
        /// Transfer wallet fund out(Tgs -> Gp)
        /// </summary>
        /// <param name="submitGameBalanceFundOutCommandService"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("fund/out")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask SubmitGameBalanceFundOutRequestAsync(
            [FromServices] ICommandService<SubmitGameBalanceFundOutCommand> submitGameBalanceFundOutCommandService,
            [FromBody] SubmitGameBalanceFundOutRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

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

        /// <summary>
        /// Transfer wallet fund in(GP -> Tgs)
        /// </summary>
        /// <param name="submitGameBalanceFundInCommandService"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("fund/in")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask SubmitGameBalanceFundInRequestAsync(
            [FromServices] ICommandService<SubmitGameBalanceFundInCommand> submitGameBalanceFundInCommandService,
            [FromBody] SubmitGameBalanceFundInRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

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
    }
}