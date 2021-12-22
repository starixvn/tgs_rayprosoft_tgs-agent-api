using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using Tgs.Payment.Clients;
using Tgs.Payment.SkyPay;
using TGS.Services.Operator.Services.Deposit;
using Tgs.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/momo")]
    [ApiController]
    public class MomoDepositController : ControllerBase
    {
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
                $"tgs.payment.skypay.{fragment.ToLower()}.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new SkyPayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("accessToken").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("gateway").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("transfer").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("exchangeRate").GetDecimal(),
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
    }
}
