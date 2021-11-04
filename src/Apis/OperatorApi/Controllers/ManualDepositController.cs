using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.ManualDeposit;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operator.Services.BrandPayments;
using TGS.Services.Operator.Services.Deposit;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/manual")]
    [ApiController]
    public class ManualDepositController : ControllerBase
    {
        [HttpGet("{id}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
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

            var paymentMethodInfo = paymentMethods[brandPayment.MethodId];

            var bankInfo = GetApplyInfo(serializer, paymentMethodInfo.BankType, form.ApplyInfo);

            if (paymentMethodInfo.BankType == "Bank")
            {
                return new ManualDepositBankFormResponse()
                {
                    MethodName = paymentMethods[brandPayment.MethodId].Name,
                    ChannelName = paymentChannels[brandPayment.ChannelId].Name,
                    Amount = form.Amount,
                    BankAccountName = ((ManualDepositBankApplyInfo)bankInfo).ReceiveAccountName,
                    BankAccount = ((ManualDepositBankApplyInfo)bankInfo).ReceiveBankAccount,
                    BankName = brandPayment.ChannelName
                };
            }
            else
            {
                return new ManualDepositThirdPartyFormResponse()
                {
                    MethodName = paymentMethods[brandPayment.MethodId].Name,
                    ChannelName = paymentChannels[brandPayment.ChannelId].Name,
                    Amount = form.Amount,
                    QrCode = ((ManualDepositThirdPartyApplyInfo)bankInfo).QrCode,
                    BankName = brandPayment.ChannelName
                };
            }
        }

        [HttpPut]
        [HttpPut("bank")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<Guid> SubmitDepositBankRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] ManualDepositSerializer serializer,
            [FromBody] SubmitManualDepositBankRequest request)
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
                    new ManualDepositBankApplyInfo()
                    {
                        BankType = request.BankType,
                        SendBankAccount = request.BankAccount,
                        ReceiveBankAccount = brandPaymentBankInfo.RootElement.GetProperty("bankAccount").GetString(),
                        ReceiveAccountName = brandPaymentBankInfo.RootElement.GetProperty("name").GetString()
                    }));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
        }

        [HttpPost("{id}")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask SubmitDepositFormAsync(
            [FromServices] ICommandService<SubmitBankDepositFormCommand> submitBankDepositFormCommandService,
            [FromServices] ManualDepositSerializer serializer,
            Guid id,
            [FromForm(Name = "file")] IFormFile manualReceipt)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var contentType = manualReceipt.ContentType;
            await using var fileStream = manualReceipt.OpenReadStream();

            var command = new SubmitBankDepositFormCommand(
                brandId,
                playerTokenData.PlayerCode,
                id,
                contentType,
                fileStream,
                id => serializer.Serialize(
                    new ManualDepositPaymentReturn(id)));

            await submitBankDepositFormCommandService.ExecuteAsync(command);
        }

        [HttpPut("thirdparty")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask<Guid> SubmitDepositThirdPartyRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] ManualDepositSerializer serializer,
            [FromBody] SubmitManualDepositThirdPartyRequest request)
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
                    new ManualDepositThirdPartyApplyInfo
                    {
                        BankType = request.BankType,
                        QrCode = brandPaymentBankInfo.RootElement.GetProperty("qrCode").GetString()
                    }));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
        }

        private ManualDepositApplyInfo GetApplyInfo(ManualDepositSerializer serializer, string bankType, string? applyInfo)
        {
            return bankType switch
            {
                "Bank" => serializer.Deserialize<ManualDepositBankApplyInfo>(applyInfo),
                "ThirdParty" => serializer.Deserialize<ManualDepositThirdPartyApplyInfo>(applyInfo),
                _ => throw new NotImplementedException()
            };
        }
    }
}
