using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.LeePay;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Payment;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Deposit;

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
            [FromServices] IGetService<GetDepositFormQuery, DepositFormDetail> getDepositFormService,
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

            var getDepositFormQuery = new GetDepositFormQuery(playerTokenData.PlayerCode, createdFormId);

            var formData = await getDepositFormService
                .GetAsync(getDepositFormQuery)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<LeePayDepositApplyInfo>(formData.ApplyInfo!);

            return new LeePayDepositFormCreatedResponse
            {
                FormId = createdFormId,
                PaymentUrl = applyInfo.Response.PaymentUrl
            };
        }
    }
}
