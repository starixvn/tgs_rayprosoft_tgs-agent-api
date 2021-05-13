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
            //[FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,            
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
            //[FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            //[FromServices] ManualDepositSerializer serializer,
            [FromBody] SubmitGameBalanceFundOutRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            //var brandId = HttpContext.Items.GetBrandId();

            //var url = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(request.GameProviderCode);
            //await _client.FundOutAsync(url, new GameProviderWalletFundRequest()
            //{
            //    PlayerCode = playerTokenData.PlayerCode,
            //    GameProviderCode = request.GameProviderCode,
            //    Amount = request.Amount
            //});
        }
    }
}