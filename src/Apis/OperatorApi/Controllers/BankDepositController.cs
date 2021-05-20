using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.LeePay;
using Tgs.Payment.PrimePay;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.Deposit;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/bank")]
    [ApiController]
    public class BankDepositController : ControllerBase
    {
        [HttpPost("primepay")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<PrimePayDepositFormCreatedResponse> SubmitDepositRequestAsync(
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
                            "https://google.com")));

            await createDepositFormService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            var createdFormId = await command.ReturnFormIdSource.Task.ConfigureAwait(false);

            var getDepositFormQuery = new GetDepositFormQuery(playerTokenData.PlayerCode, createdFormId);

            var formData = await getDepositFormService
                .GetAsync(getDepositFormQuery)
                .ConfigureAwait(false);

            var applyInfo = serializer.Deserialize<PrimePayDepositApplyInfo>(formData.ApplyInfo!);

            return new PrimePayDepositFormCreatedResponse
            {
                FormId = createdFormId,
                PaymentUrl = ((PrimePayDepositSuccessResponse)applyInfo.Response).RedirectUrl.ToString()
            };
        }
    }
}
