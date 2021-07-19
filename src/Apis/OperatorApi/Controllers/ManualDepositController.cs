using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operator.Services.BrandPayments;
using TGS.Services.Operator.Services.Deposit;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/manual")]
    [ApiController]
    public class ManualDepositController : ControllerBase
    {
        [HttpGet("{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async Task<ManualDepositFormResponse> GetDepositFormAsync(
            [FromServices] IGetService<GetBankDepositFormQuery, DepositFormInfo> getBankDepositService,
            [FromServices]
            IGetService<AllPaymentChannelsQuery, IDictionary<Guid, PaymentChannelInfo>> getAllPaymentChannelService,
            [FromServices]
            IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethodInfo>> getAllPaymentMethodService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] ManualDepositSerializer serializer,
            Guid id)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var getBankDepositFormQuery = new GetBankDepositFormQuery(
                playerTokenData.PlayerCode,
                id,
                brandId);

            var paymentChannels = await getAllPaymentChannelService
                .GetAsync(new())
                .ConfigureAwait(false);
            var paymentMethods = await getAllPaymentMethodService
                .GetAsync(new())
                .ConfigureAwait(false);

            var form = await getBankDepositService
                .GetAsync(getBankDepositFormQuery)
                .ConfigureAwait(false);

            var brandPayment = await getBrandPaymentService
                .GetAsync(new(form.BrandPaymentId))
                .ConfigureAwait(false);

            var bankInfo = serializer.Deserialize<ManualDepositApplyInfo>(form.ApplyInfo);

            return new()
            {
                MethodName = paymentMethods[brandPayment.MethodId].Name,
                ChannelName = paymentChannels[brandPayment.ChannelId].Name,
                Amount = form.Amount,
                BankAccountName = bankInfo.ReceiveAccountName,
                BankAccount = bankInfo.ReceiveBankAccount,
                BankName = brandPayment.ChannelName
            };
        }

        [HttpPost("{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async ValueTask SubmitDepositFormAsync(
            [FromServices] ICommandService<SubmitBankDepositFormCommand> submitBankDepositFormCommandService,
            Guid id,
            [FromForm(Name = "file")] IFormFile manualReceipt)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var contentType = manualReceipt.ContentType;
            await using var fileStream = manualReceipt.OpenReadStream();

            var command = new SubmitBankDepositFormCommand(
                playerTokenData.PlayerCode,
                id,
                contentType,
                fileStream);

            await submitBankDepositFormCommandService.ExecuteAsync(command);
        }

        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async ValueTask<Guid> SubmitDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] ManualDepositSerializer serializer,
            [FromBody] SubmitManualDepositRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentBankInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                "tgs.payment.manual.deposit",
                request.PromotionId,
                _ => serializer.Serialize(
                    new ManualDepositApplyInfo(
                        request.BankAccount,
                        brandPaymentBankInfo.RootElement.GetProperty("bankAccount").GetString(),
                        brandPaymentBankInfo.RootElement.GetProperty("name").GetString())));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
        }
    }
}
