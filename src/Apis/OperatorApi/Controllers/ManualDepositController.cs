using System;
using System.Collections.Generic;
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
using TGS.Services.Operator.Deposit;
using TGS.Services.Payment;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/manual")]
    [ApiController]
    public class ManualDepositController : ControllerBase
    {
        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask<Guid> SubmitDepositRequestAsync(
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

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
        }

        [HttpGet("{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<DepositFormResponse> GetDepositFormAsync(
            [FromServices] IGetService<GetBankDepositFormQuery, DepositFormInfo> getBankDepositService,
            [FromServices] IGetService<AllPaymentChannelsQuery, IDictionary<Guid, PaymentChannelInfo>> getAllPaymentChannelService,
            [FromServices] IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethodInfo>> getAllPaymentMethodService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
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

        [HttpPost("{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask SubmitDepositFormAsync(
            [FromServices] ICommandService<SubmitBankDepositFormCommand> submitBandDepositFormCommandService,
            Guid id,
            [FromForm(Name = "file")] IFormFile manualReceipt)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var contentType = manualReceipt.ContentType;
            using var fileStream = manualReceipt.OpenReadStream();

            var command = new SubmitBankDepositFormCommand(
                playerTokenData.PlayerCode,
                id,
                contentType,
                fileStream);

            await submitBandDepositFormCommandService.ExecuteAsync(command);
        }
    }
}
