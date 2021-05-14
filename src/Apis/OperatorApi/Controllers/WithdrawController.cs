using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operator;
using TGS.Services.Operator.Deposit;
using TGS.Services.Operator.Withdraw;
using TGS.Services.Payment;
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

        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async IAsyncEnumerable<BrandPaymentResponse> GetBrandPaymentsAsync(
            [FromServices] IQueryService<BrandPaymentsQuery, BrandPaymentInfo> brandPaymentsQueryService,
            [FromServices] IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethodInfo>> getAllPaymentMethodService,
            [FromServices] IGetService<GetPaymentQuery, PaymentInfo> getPaymentService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var paymentMethods = await getAllPaymentMethodService
                .GetAsync(new AllPaymentMethodsQuery())
                .ConfigureAwait(false);

            var query = new BrandPaymentsQuery(brandId, playerTokenData.PlayerCode);

            var brandPayments = await brandPaymentsQueryService.QueryAsync(query)
                .ToArrayAsync()
                .ConfigureAwait(false);

            foreach (var bp in brandPayments)
            {
                if (bp.TransferType != TransferType.Withdraw)
                    continue;

                var getPaymentQuery = new GetPaymentQuery(bp.PaymentId);

                var paymentInfo = await getPaymentService
                    .GetAsync(getPaymentQuery)
                    .ConfigureAwait(false);

                var paymentMethod = paymentMethods[bp.MethodId];

                yield return new BrandPaymentResponse
                {
                    Id = bp.Id,
                    ChannelId = bp.ChannelId,
                    ChannelName = bp.ChannelName,
                    TransferLimit = new Models.ValueRange<decimal>
                    {
                        From = bp.TransferLimit.From,
                        To = bp.TransferLimit.To
                    },
                    ExtendedData = JsonDocument.Parse(bp.ExtendedData),
                    PaymentId = bp.PaymentId,
                    PaymentCode = paymentInfo.Code,
                    MethodId = bp.MethodId,
                    MethodCode = paymentMethod.Name,
                    BankType = paymentMethod.BankType
                };
            }
        }
    }
}
