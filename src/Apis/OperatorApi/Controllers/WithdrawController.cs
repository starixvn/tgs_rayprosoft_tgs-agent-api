using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operator;
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
            [FromServices] ICommandService<CheckWithdrawPayPasswordCommand> checkPasswordService,
            CreateWithdrawRequest request)
        {
            try
            {
                var brandId = HttpContext.Items.GetBrandId();
                var playerData = HttpContext.Items.GetPlayerTokenData();

                var checkPasswordCommand = new CheckWithdrawPayPasswordCommand(playerData.PlayerCode, request.PayPassword);
                await checkPasswordService
                    .ExecuteAsync(checkPasswordCommand)
                    .ConfigureAwait(false);

                if (!await checkPasswordCommand.ReturnSource.Task.ConfigureAwait(false))
                    throw new OperationNotAllowedException();

                var command = new CreateWithdrawFormCommand(
                    playerData.PlayerCode,
                    brandId,
                    request.BankInfoId,
                    request.Amount);

                await createFormService.ExecuteAsync(command).ConfigureAwait(false);

                return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
            }
            catch (InsufficientFundsException)
            {
                throw new PlayerHasInsufficientFundsException();
            }
        }
    }
}
