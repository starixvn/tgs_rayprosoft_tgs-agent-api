using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.Withdraw;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<Guid> CreateWithdrawRequestAsync(
            [FromServices] ICommandService<CreateWithdrawFormCommand> createFormService,
            CreateWithdrawRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerData = HttpContext.Items.GetPlayerTokenData();

            var command = new CreateWithdrawFormCommand(
                playerData.PlayerCode,
                brandId,
                request.BankInfoId,
                request.Amount);

            await createFormService.ExecuteAsync(command).ConfigureAwait(false);

            return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
        }
    }
}
