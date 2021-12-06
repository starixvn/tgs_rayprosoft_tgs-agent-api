using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay;
using Tgs.Payment.MaticPay;
using Tgs.Payment.PrimePay;
using Tgs.Payment.SkyPay;
using Tgs.Payment.UmPay;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Deposit;
using TGS.Services.Operator.Services.Player;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/bank")]
    [ApiController]
    public class BankDepositController : ControllerBase
    {
        private static readonly Random _Random = new Random();

        [HttpGet("help2pay/pay/{identityId}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        public async ValueTask<ContentResult> GetHelp2PayDepositHtmlAsync(
            [FromServices] IHelp2PayRepository redisRepository,
            [FromRoute] Guid identityId)
        {
            var html = await redisRepository.GetDepositHtmlAsync(identityId)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(html))
            {
                throw new PaymentFailException($"Can not find help2pay deposit html of identity id:{identityId}");
            }

            return new ContentResult
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = "text/html",
                Content = html
            };
        }

        [HttpPost("help2pay")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<DepositFormCreatedResponse> SubmitHelp2PayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SubmitDepositFormThirdPartyOrderNoCommand> updateDepositFormService,
            [FromServices] IHelp2PayRepository redisRepository,
            [FromServices] Help2PaySerializer serializer,
            [FromBody] SubmitDepositRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData!);

            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                "tgs.payment.help2pay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new Help2PayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("merchantId").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("securityCode").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("currency").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("bankCode").GetString(),
                        formId,
                        "https://google.com",
                        clientIp,
                        brandPaymentInfo.RootElement.GetProperty("exchangeRate").GetDecimal())));

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

            var applyInfo = serializer.Deserialize<Help2PayDepositApplyInfo>(formData.ApplyInfo!);

            if (!string.IsNullOrWhiteSpace(applyInfo.Response.PaymentDepositId))
            {
                var updateThirdPartyOrderNoCommand = new SubmitDepositFormThirdPartyOrderNoCommand(
                    createdFormId,
                    applyInfo.Response.PaymentDepositId);

                await updateDepositFormService
                    .ExecuteAsync(updateThirdPartyOrderNoCommand)
                    .ConfigureAwait(false);

                var identityId = Guid.NewGuid();

                await redisRepository.StoreDepositHtmlAsync(identityId, applyInfo.Response.Html)
                    .ConfigureAwait(false);

                return new DepositFormCreatedResponse
                {
                    FormId = createdFormId,
                    PaymentUrl = $"/api/deposit/bank/help2pay/pay/{identityId}"
                };
            }

            throw new PaymentFailException("Something went wrong to deposit via help2pay");
        }

        [HttpPost("maticpay")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<MaticPayDepositFormCreatedResponse> SubmitMaticPayDepositRequestAsync(
           [FromServices] IGetService<GetPlayerQuery, PlayerDataInfo> getPlayerInfoService,
           [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
           [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
           [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
           [FromServices] ICommandService<SubmitDepositFormThirdPartyOrderNoCommand> updateDepositFormService,
           [FromServices] MaticPaySerializer serializer,
           [FromBody] SubmitDepositRequest request
           )
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var player = await getPlayerInfoService.GetAsync(new GetPlayerQuery(brandId, playerTokenData.PlayerCode));

            if (!player.IdentityAuthentication)
                throw new MaticPayDepositNeedRealNameException();

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

                var payUri = new Uri(paymentUrl);
                var thirdPartyOrderNo = HttpUtility.ParseQueryString(payUri.Query).Get("oid");

                var updateThirdPartyOrderNoCommand = new SubmitDepositFormThirdPartyOrderNoCommand(
                    createdFormId,
                    thirdPartyOrderNo);

                await updateDepositFormService
                    .ExecuteAsync(updateThirdPartyOrderNoCommand)
                    .ConfigureAwait(false);

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
        public async Task<DepositFormCreatedResponse> SubmitPrimePayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SubmitDepositFormThirdPartyOrderNoCommand> updateDepositFormService,
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
            {
                var updateThirdPartyOrderNoCommand = new SubmitDepositFormThirdPartyOrderNoCommand(
                    createdFormId,
                    ((PrimePayDepositSuccessResponse)applyInfo.Response).ReferenceId);

                await updateDepositFormService
                    .ExecuteAsync(updateThirdPartyOrderNoCommand)
                    .ConfigureAwait(false);

                return new DepositFormCreatedResponse
                {
                    FormId = createdFormId,
                    PaymentUrl = ((PrimePayDepositSuccessResponse)applyInfo.Response).RedirectUrl.ToString()
                };
            }
            else
            {
                var failResponse = (PrimePayDepositFailResponse)applyInfo.Response;

                throw new PaymentFailException(failResponse.Errors);
            }
        }

        [HttpPost("skypay-{fragment}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [PlayerCheckEnabled]
        public async Task<DepositFormCreatedResponse> SubmitSkyPayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] SkyPaySerializer serializer,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SubmitDepositFormThirdPartyOrderNoCommand> updateDepositFormService,
            string fragment,
            [FromBody] SubmitDepositRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.GetBrandId().Value;

            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(request.BrandPaymentId))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var command = new CreateDepositFormCommand(
                playerTokenData.PlayerCode,
                brandId,
                request.BrandPaymentId,
                request.Amount,
                $"tgs.payment.skypay.{fragment}.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new SkyPayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("accessToken").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("gateway").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("transfer").GetString(),
                        formId)));

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

            var applyInfo = serializer.Deserialize<SkyPayDepositApplyInfo>(formData.ApplyInfo!);

            var updateThirdPartyOrderNoCommand = new SubmitDepositFormThirdPartyOrderNoCommand(
                    createdFormId,
                    applyInfo.Response.TradeNo);

            await updateDepositFormService
                .ExecuteAsync(updateThirdPartyOrderNoCommand)
                .ConfigureAwait(false);

            return new DepositFormCreatedResponse
            {
                FormId = createdFormId,
                PaymentUrl = applyInfo.Response.TradeUrl
            };
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
            [FromBody] SubmitDepositRequest request
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
                        1m,
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
