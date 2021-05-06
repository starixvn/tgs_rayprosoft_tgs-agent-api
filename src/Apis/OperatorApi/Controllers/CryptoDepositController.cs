using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.LeePay;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Json;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operator.Deposit;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/deposit/crypto")]
    [ApiController]
    public class CryptoDepositController : ControllerBase
    {
        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async ValueTask<Guid> SubmitDepositRequestAsync(
            [FromServices] ICommandService<CreateDepositFormCommand> createDepositFormService,
            [FromServices] IGetService<GetBrandPaymentQuery, BrandPaymentInfo> getBrandPaymentService,
            [FromServices] LeePaySerializer serializer,
            [FromBody] SubmitCryptoDepositRequest request)
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

            return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
        }
    }
}
