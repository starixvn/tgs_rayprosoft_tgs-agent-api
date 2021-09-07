using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Models.Exceptions;
using Tgs.Payment.Clients;
using TGS.Services.Agent.Exceptions;
using TGS.Services.Agent.Services.BrandPayments;
using TGS.Services.Agent.Services.Withdraw;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        [HttpPost]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        [CheckEnabledAttribute]
        public async Task<Guid> CreateWithdrawRequestAsync(
            [FromServices] ICommandService<CreateWithdrawFormCommand> createFormService,
            [FromServices] ICommandService<CheckWithdrawPayPasswordCommand> checkPasswordService,
            CreateWithdrawRequest request)
        {
            try
            {
                var brandId = HttpContext.Items.GetBrandId();
                var agentData = HttpContext.Items.GetAgentTokenData();

                var checkPasswordCommand = new CheckWithdrawPayPasswordCommand(agentData.AgentId, request.PayPassword);
                await checkPasswordService
                    .ExecuteAsync(checkPasswordCommand)
                    .ConfigureAwait(false);

                if (!await checkPasswordCommand.ReturnSource.Task.ConfigureAwait(false))
                    throw new OperationNotAllowedException();

                var command = new CreateWithdrawFormCommand(
                    agentData.AgentId,
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
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async IAsyncEnumerable<BrandPaymentResponse> GetBrandPaymentsAsync(
            [FromServices] IQueryService<BrandPaymentsQuery, BrandPaymentInfo> brandPaymentsQueryService,
            [FromServices]
            IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethodInfo>> getAllPaymentMethodService,
            [FromServices] IGetService<GetPaymentQuery, PaymentInfo> getPaymentService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var paymentMethods = await getAllPaymentMethodService
                .GetAsync(new AllPaymentMethodsQuery())
                .ConfigureAwait(false);

            var query = new BrandPaymentsQuery(brandId);

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

                if (paymentInfo.Code == "Manual")
                    continue;

                var paymentMethod = paymentMethods[bp.MethodId];

                yield return new BrandPaymentResponse
                {
                    Id = bp.Id,
                    ChannelId = bp.ChannelId,
                    ChannelName = bp.ChannelName,
                    ChannelCode = bp.ChannelCode,
                    TransferLimit = new ValueRange<decimal>
                    {
                        From = bp.TransferLimit.From,
                        To = bp.TransferLimit.To
                    },
                    ExtendedData = JsonDocument.Parse("{}"),
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