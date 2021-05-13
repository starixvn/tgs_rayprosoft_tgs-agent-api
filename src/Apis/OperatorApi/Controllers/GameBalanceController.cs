using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
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
        public GameBalanceController()
        {
        }

        [HttpGet("{gpCode}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<GetGameBalanceResponse> GetGameBalanceAsync(
            [FromServices] IGetService<GetGameBalanceQuery, GameBalanceInfo> getGameBalanceService,
            string gpCode,
            string playerCode)
        {
            var query = new GetGameBalanceQuery(playerCode, gpCode);

            var info = await getGameBalanceService.GetAsync(query).ConfigureAwait(false);

            return new GetGameBalanceResponse
            {
                Balance = info.balance
            };
        }

        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask SubmitGameBalanceFundInRequestAsync(
            [FromServices] ICommandService<SubmitGameBalanceFundInCommand> submitGameBalanceFundInCommandService,
            [FromBody] SubmitGameBalanceFundInRequest request)
        {
            var command = new SubmitGameBalanceFundInCommand
            (
                request.PlayerCode,
                request.GameProviderCode,
                request.Amount
            );

            await submitGameBalanceFundInCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }

        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask SubmitGameBalanceFundOutRequestAsync(
            [FromServices] ICommandService<SubmitGameBalanceFundOutCommand> submitGameBalanceFundOutCommandService,
            [FromBody] SubmitGameBalanceFundOutRequest request)
        {
            var command = new SubmitGameBalanceFundOutCommand
            (
                request.PlayerCode,
                request.GameProviderCode,
                request.Amount
            );

            await submitGameBalanceFundOutCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }
    }
}