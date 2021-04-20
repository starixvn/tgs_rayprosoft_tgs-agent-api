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
using TGS.Services.Operator.BankDeposit;
using TGS.Services.Operator.Deposit;
using TGS.Services.Payment;
using TGS.ServiceUtility;

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
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            [FromBody] SubmitDepositRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentBankInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var command = new CreateBankDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                jsonSerializationProvider.SerializeToString(new
                {
                    SendBankAccount = request.BankAccount,
                    ReceiveBankAccount = brandPaymentBankInfo.RootElement.GetProperty("bankAccount").GetString(),
                    ReceiveAccountName = brandPaymentBankInfo.RootElement.GetProperty("name").GetString()
                }));

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
