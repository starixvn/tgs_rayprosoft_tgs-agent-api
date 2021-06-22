using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment;
using Tgs.Payment.Clients;
using Tgs.Payment.LeePay;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Deposit;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/crypto")]
    [ApiController]
    public class CryptoDepositController : ControllerBase
    {
        [HttpPut("leepay")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask<LeePayDepositFormCreatedResponse> SubmitDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] LeePaySerializer serializer,
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
                "tgs.payment.leepay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new LeePayDepositSettings(
                            brandPaymentInfo.RootElement.GetProperty("systemId").GetString(),
                            brandPaymentInfo.RootElement.GetProperty("systemKey").GetString(),
                            formId,
                            brandPaymentInfo.RootElement.GetProperty("channelCurrency").GetString(),
                            "http://localhost",
                            brandPaymentInfo.RootElement.GetProperty("channel").GetString())));

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

            var applyInfo = serializer.Deserialize<LeePayDepositApplyInfo>(formData.ApplyInfo!);

            if (applyInfo.Response.Code != "00")
                throw new PaymentFailException(applyInfo.Response.Message);

            return new LeePayDepositFormCreatedResponse
            {
                FormId = createdFormId,
                PaymentUrl = applyInfo.Response.PaymentUrl
            };
        }

        [HttpGet("leepay/exchangerate/{brandPaymentId}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<GetLeePayExchangeRateResponse> GetExchangeRateAsync(
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] IGetService<GetLeePayExchangeRateQuery, string> getLeePayExchangeRateService,
            [FromServices] LeePaySerializer serializer,
            string brandPaymentId)
        {
            var brandPayment = await getBrandPaymentService
                .GetAsync(new GetBrandPaymentQuery(new Guid(brandPaymentId)))
                .ConfigureAwait(false);

            var brandPaymentInfo = JsonDocument.Parse(brandPayment.ExtendedData);

            var query = new GetLeePayExchangeRateQuery(
                "tgs.payment.leepay.queryexchangerate",
                serializer.Serialize(
                    new LeePayQueryExchangeRateSettings(
                        brandPaymentInfo.RootElement.GetProperty("systemId").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("systemKey").GetString(),
                        "CNY",
                        "USDT")));

            var response = await getLeePayExchangeRateService
                .GetAsync(query)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<LeePayQueryExchangeRateApplyInfo>(response);

            if (applyInfo.Response.Code == "00")
            {
                return new GetLeePayExchangeRateResponse
                {
                    BuyPrice = applyInfo.Response.BuyPrice.Value.ToString("#.#####"),
                    SellPrice = applyInfo.Response.SellPrice.Value.ToString("#.#####"),
                    UpdateTime = applyInfo.Response.BizTime
                };
            }
            else
            {
                throw new LeePayApiException();
            }
        }
    }
}