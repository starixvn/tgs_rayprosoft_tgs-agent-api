using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.ApplePay;
using Tgs.Payment.Clients;
using Tgs.Payment.MaticPay;
using Tgs.Payment.PrimePay;
using Tgs.Payment.UmPay;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Deposit;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/bank")]
    [ApiController]
    public class BankDepositController : ControllerBase
    {
        private static Random _Random = new Random();

        [HttpPost("applepay")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [PlayerCheckEnabled]
        public async Task<ApplePayDepositFormCreatedResponse> SubmitApplePayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ApplePaySerializer serializer,
            [FromBody] SubmitApplePayDepositRequest request
            )
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                "tgs.payment.applepay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new ApplePayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("apiKey").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("storeGuid").GetString(),
                        formId,
                        brandPaymentInfo.RootElement.GetProperty("type").GetString(),
                        request.TransName)));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            var createdFormId = await command.ReturnFormIdSource.Task.ConfigureAwait(false);

            var getDepositFormQuery = new GetDepositFormQuery(
                playerTokenData.PlayerCode,
                createdFormId,
                brandId);

            var formData = await getDepositFormService
                .GetAsync(getDepositFormQuery)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<ApplePayDepositApplyInfo>(formData.ApplyInfo!);

            if (applyInfo.Response.Result == 1)
            {
                return new ApplePayDepositFormCreatedResponse
                {
                    FormId = createdFormId,
                    PaymentUrl = applyInfo.Response.ReceiveUrl.ToString(),
                    ErrorMessage = string.Empty
                };
            }
            else
            {
                throw new PaymentFailException(applyInfo.Response.Msg);
            }
        }

        [HttpPost("maticpay")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<MaticPayDepositFormCreatedResponse> SubmitMaticPayDepositRequestAsync(
           [FromServices] IGetService<GetPlayerQuery, PlayerDataInfo> getPlayerInfoService,
           [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
           [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
           [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
           [FromServices] MaticPaySerializer serializer,
           [FromBody] SubmitMaticPayDepositRequest request
           )
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var player = await getPlayerInfoService.GetAsync(new GetPlayerQuery(brandId, playerTokenData.PlayerCode));

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                "tgs.payment.maticpay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new MaticPayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("merchantNo").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("apiKey").GetString(),
                        formId,
                        request.Amount,
                        player.RealName)));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            var createdFormId = await command.ReturnFormIdSource.Task.ConfigureAwait(false);

            var getDepositFormQuery = new GetDepositFormQuery(
                playerTokenData.PlayerCode,
                createdFormId,
                brandId);

            var formData = await getDepositFormService
                .GetAsync(getDepositFormQuery)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<MaticPayDepositApplyInfo>(formData.ApplyInfo!);

            if (applyInfo.Response.Code == "0")
            {
                var paymentUrl = applyInfo.Response.Detail.PayURL.ToString();
                if (applyInfo.Response.Detail.PayURL.ToString() == string.Empty)
                {
                    paymentUrl = applyInfo.Response.Detail.PayHtml.ToString();
                }

                return new MaticPayDepositFormCreatedResponse
                {
                    FormId = createdFormId,
                    PaymentUrl = paymentUrl,
                    ErrorMessage = string.Empty
                };
            }
            else
            {
                throw new PaymentFailException(applyInfo.Response.Msg);
            }
        }

        [HttpPost("primepay")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [PlayerCheckEnabled]
        public async Task<PrimePayDepositFormCreatedResponse> SubmitPrimePayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] PrimePaySerializer serializer,
            [FromBody] SubmitDepositRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                "tgs.payment.primepay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new PrimePayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("merchantId").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("key").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("iv").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("currency").GetString(),
                        formId,
                        "https://google.com",
                        brandPaymentInfo.RootElement.TryGetProperty("paymentMethod", out var prop)
                            ? prop.GetString()
                            : null)));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            var createdFormId = await command.ReturnFormIdSource.Task.ConfigureAwait(false);

            var getDepositFormQuery = new GetDepositFormQuery(
                playerTokenData.PlayerCode,
                createdFormId,
                brandId);

            var formData = await getDepositFormService
                .GetAsync(getDepositFormQuery)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<PrimePayDepositApplyInfo>(formData.ApplyInfo!);

            if (applyInfo.Response is PrimePayDepositSuccessResponse response)
                return new PrimePayDepositFormCreatedResponse
                {
                    FormId = createdFormId,
                    PaymentUrl = ((PrimePayDepositSuccessResponse)applyInfo.Response).RedirectUrl.ToString()
                };
            else
            {
                var failResponse = (PrimePayDepositFailResponse)applyInfo.Response;

                throw new PaymentFailException(failResponse.Errors);
            }
        }

        [HttpPost("umpay")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<UmPayDepositFormCreatedResponse> SubmitUmPayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SubmitDepositFormThirdPartyOrderNoCommand> updateDepositFormService,
            [FromServices] UmPaySerializer serializer,
            [FromBody] SubmitUmPayDepositRequest request
            )
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            //需要隨機兩位小數的通道類型:PAY01支轉支 PAY03轉數快 PAY04網銀
            var randomPointTradeTypes = new string[] { "PAY01", "PAY03", "PAY04" };

            var tradeType = brandPaymentInfo.RootElement.GetProperty("tradeType").GetString();
            var amount = request.Amount;
            if (randomPointTradeTypes.Contains(tradeType))
            {
                amount += _Random.Next(1, 100) / 100M;
            }

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                amount,
                "tgs.payment.umpay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new UmPayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("apiKey").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("merchantNo").GetString(),
                        tradeType,
                        "1",
                        formId,
                        amount,
                        Rate: 1m,
                        formId.ToString())));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            var createdFormId = await command.ReturnFormIdSource.Task.ConfigureAwait(false);

            var getDepositFormQuery = new GetDepositFormQuery(
                playerTokenData.PlayerCode,
                createdFormId,
                brandId);

            var formData = await getDepositFormService
                .GetAsync(getDepositFormQuery)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<UmPayDepositApplyInfo>(formData.ApplyInfo!);

            if (applyInfo.Response.Code == "1")
            {
                var updateThirdPartyOrderNoCommand = new SubmitDepositFormThirdPartyOrderNoCommand(
                    createdFormId,
                    applyInfo.Response.TradeNo);

                await updateDepositFormService
                    .ExecuteAsync(updateThirdPartyOrderNoCommand)
                    .ConfigureAwait(false);

                return new UmPayDepositFormCreatedResponse
                {
                    FormId = createdFormId,
                    PaymentUrl = applyInfo.Response.JumpUrl.ToString(),
                    ErrorMessage = string.Empty
                };
            }
            else
            {
                throw new PaymentFailException(applyInfo.Response.Msg);
            }
        }
    }
}
