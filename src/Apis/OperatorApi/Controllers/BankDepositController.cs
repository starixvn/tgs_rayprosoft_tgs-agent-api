using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.BankDeposit;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/bank")]
    [ApiController]
    public class BankDepositController : ControllerBase
    {
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public ValueTask SubmitDepositRequestAsync(
            [FromServices] ICommandService<CreateBankDepositFormCommand> createDepositFormService,
            [FromBody] SubmitDepositRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var command = new CreateBankDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.BankAccount,
                request.Amount);

            return createDepositFormService.ExecuteAsync(command);
        }
    }
}
