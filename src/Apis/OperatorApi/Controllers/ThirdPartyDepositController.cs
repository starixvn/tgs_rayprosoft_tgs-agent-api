using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.MalakiPay;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.ApplicationServices.Deposit;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/thirdparty")]
    [ApiController]
    public class ThirdPartyDepositController : ControllerBase
    {
        [HttpPost("malakipay", Name = "SubmitMalakiPayDepositRequest")]
        [HttpPost("~/api/deposit/bank/malakipay", Name = "SubmitMalakiPayBankDepositRequest")]
        [PlayerCheckEnabled]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<DepositFormCreatedResponse> SubmitMalakiPayDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] MalakiPaySerializer serializer,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SubmitDepositFormThirdPartyOrderNoCommand> updateDepositFormService,
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
                "tgs.payment.malakipay.deposit",
                request.PromotionId,
                formId => serializer.Serialize(
                    new MalakiPayDepositSettings(
                        brandPaymentInfo.RootElement.GetProperty("merchant").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("secret").GetString(),
                        brandPaymentInfo.RootElement.GetProperty("payType").GetString(),
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

            var applyInfo = serializer.Deserialize<MalakiPayDepositApplyInfo>(formData.ApplyInfo!);

            var updateThirdPartyOrderNoCommand = new SubmitDepositFormThirdPartyOrderNoCommand(
                createdFormId,
                applyInfo.Response.TradeNo);

            await updateDepositFormService
                .ExecuteAsync(updateThirdPartyOrderNoCommand)
                .ConfigureAwait(false);

            return new DepositFormCreatedResponse
            {
                FormId = createdFormId,
                PaymentUrl = applyInfo.Response.RedirectUrl.ToString()
            };
        }
    }
}
