using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator.Services.Deposit;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.BrandPayments;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        /// <summary>
        /// Gets the deposit brand payments.
        /// </summary>
        /// <param name="brandPaymentsQueryService">The brand payments query service.</param>
        /// <param name="getAllPaymentMethodService">The get all payment method service.</param>
        /// <param name="getPaymentService">The get payment service.</param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async IAsyncEnumerable<BrandPaymentResponse> GetBrandPaymentsAsync(
            [FromServices] IQueryService<BrandPaymentsQuery, BrandPaymentInfo> brandPaymentsQueryService,
            [FromServices] IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethodInfo>> getAllPaymentMethodService,
            [FromServices] IGetService<GetPaymentQuery, PaymentInfo> getPaymentService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var paymentMethods = await getAllPaymentMethodService
                .GetAsync(new AllPaymentMethodsQuery())
                .ConfigureAwait(false);

            var query = new BrandPaymentsQuery(brandId, playerTokenData.PlayerCode);

            var brandPayments = await brandPaymentsQueryService.QueryAsync(query)
                .ToArrayAsync()
                .ConfigureAwait(false);

            foreach (var bp in brandPayments)
            {
                if (bp.TransferType != TransferType.Deposit)
                    continue;

                var getPaymentQuery = new GetPaymentQuery(bp.PaymentId);

                var paymentInfo = await getPaymentService
                    .GetAsync(getPaymentQuery)
                    .ConfigureAwait(false);

                var paymentMethod = paymentMethods[bp.MethodId];

                yield return new BrandPaymentResponse
                {
                    Id = bp.Id,
                    ChannelId = bp.ChannelId,
                    ChannelName = bp.ChannelName,
                    ChannelCode = bp.ChannelCode,
                    TransferLimit = new Models.ValueRange<decimal>
                    {
                        From = bp.TransferLimit.From,
                        To = bp.TransferLimit.To
                    },
                    ExtendedData = paymentInfo.Code == "Manual"
                        ? JsonDocument.Parse(bp.ExtendedData)
                        : JsonDocument.Parse("{}"),
                    PaymentId = bp.PaymentId,
                    PaymentCode = paymentInfo.Code,
                    MethodId = bp.MethodId,
                    MethodCode = paymentMethod.Name,
                    BankType = paymentMethod.BankType
                };
            }
        }
    }
}
