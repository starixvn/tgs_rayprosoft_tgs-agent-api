using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Operator;
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

        [HttpGet("{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async Task<DepositFormResponse> GetDepositFormAsync(
            [FromServices] IGetService<GetBankDepositFormQuery, BankDepositForm> getBankDepositService,
            [FromServices] IGetService<AllPaymentChannelsQuery, IDictionary<Guid, PaymentChannel>> getAllPaymentChannelService,
            [FromServices] IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethod>> getAllPaymentMethodService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPayment> getBrandPaymentService,
            [FromServices] IJsonSerializationProvider serializationProvider,
            Guid id)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var getBankDepositFormQuery = new GetBankDepositFormQuery(playerTokenData.PlayerCode, id);

            var paymentChannels = await getAllPaymentChannelService
                .GetAsync(new AllPaymentChannelsQuery())
                .ConfigureAwait(false);
            var paymentMethods = await getAllPaymentMethodService
                .GetAsync(new AllPaymentMethodsQuery())
                .ConfigureAwait(false);

            var form = await getBankDepositService
                .GetAsync(getBankDepositFormQuery)
                .ConfigureAwait(false);

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(form.BrandPaymentId))
                .ConfigureAwait(false);

            var bankInfo = serializationProvider.DeserializeFromString<Dictionary<string, string>>(brandPayment.ExtendedData);

            return new DepositFormResponse
            {
                MethodName = paymentMethods[brandPayment.MethodId].Name,
                ChannelName = paymentChannels[brandPayment.ChannelId].Name,
                Amount = form.Amount,
                BankAccountName = bankInfo["name"].ToString(),
                BankAccount = bankInfo["bankAccount"].ToString(),
                BankName = brandPayment.ChannelName
            };
        }
    }
}
