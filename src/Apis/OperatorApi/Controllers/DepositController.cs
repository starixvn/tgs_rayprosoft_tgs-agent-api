using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Apis.OperatorApi.Models.Messages.Deposit;
using TGS.Core.Models.Messages;
using TGS.Services.Operator.Services.BrandPayments;
using TGS.Services.Operator.Services.Deposit;
using TGS.ServiceUtility;

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

        /// <summary>
        /// Get the player deposit info list.
        /// </summary>
        /// <param name="getDepositFormsQueryService"></param>
        /// <param name="request"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet("player/list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<PagedResponse<DepositFormViewModel>> GetDepositFormsAsync(
            [FromServices] IGetService<GetDepositFormsQuery, Tgs.Payment.Clients.PagingResponse<DepositFormInfo>> getDepositFormsQueryService,
            [FromQuery] GetDepositFormsRequest request,
            [FromQuery] PageableRequest paging)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var createOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;
            var updateOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;

            if (request.BeginCreateOn != null && request.EndCreateOn != null)
                createOnRange = (request.BeginCreateOn.Value, request.EndCreateOn.Value);

            if (request.BeginUpdateOn != null && request.EndUpdateOn != null)
                updateOnRange = (request.BeginUpdateOn.Value, request.EndUpdateOn.Value);

            var query = new GetDepositFormsQuery(
                brandId,
                playerTokenData.PlayerCode,
                request.Status.ToPaymentFlowState(),
                createOnRange,
                updateOnRange,
                paging);

            var result = await getDepositFormsQueryService.GetAsync(query).ConfigureAwait(false);

            var response = new PagedResponse<DepositFormViewModel>
            {
                Valid = true,
                TotalCount = result.TotalCount,
                Items = result.Items
                    .Select(form => new DepositFormViewModel
                    {
                        Id = form.Id,
                        Status = form.Status.ToFlowState(),
                        Amount = form.Amount,
                        CreatedOn = form.CreatedOn,
                        UpdatedOn = form.UpdatedOn
                    })
                    .ToArray()
            };
            return response;
        }

        [HttpGet("status")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public Task<Dictionary<int, string>> GetDepositFormStatusAsync()
        {
            var dict = Enum.GetValues(typeof(FlowState))
               .Cast<FlowState>()
               .Where(x => x != FlowState.Unknown)
               .ToDictionary(t => (int)t, t => t.ToString());
            return Task.FromResult(dict);
        }
    }
}