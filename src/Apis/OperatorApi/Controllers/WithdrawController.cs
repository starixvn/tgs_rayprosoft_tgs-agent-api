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
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Messages;
using TGS.Services.Operator.BrandPayment;
using TGS.Services.Operator.Services.BrandPayments;
using TGS.Services.Operator.Services.Withdraw;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [PlayerCheckEnabled]
        public async Task<Guid> CreateWithdrawRequestAsync(
            [FromServices] ICommandService<CreateWithdrawFormCommand> createFormService,
            [FromServices] ICommandService<CheckWithdrawPayPasswordCommand> checkPasswordService,
            CreateWithdrawRequest request)
        {
            try
            {
                var brandId = HttpContext.Items.GetBrandId();
                var playerData = HttpContext.Items.GetPlayerTokenData();

                var command = new CreateWithdrawFormCommand(
                    playerData.PlayerCode,
                    brandId,
                    request.BankInfoId,
                    request.Amount);

                await createFormService.ExecuteAsync(command).ConfigureAwait(false);

                return await command.ReturnFormIdSource.Task.ConfigureAwait(false);
            }
            catch (InsufficientFundsException)
            {
                throw new PlayerHasInsufficientFundsException();
            }
        }

        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async IAsyncEnumerable<BrandPaymentResponse> GetBrandPaymentsAsync(
            [FromServices] IQueryService<BrandPaymentsQuery, BrandPaymentInfo> brandPaymentsQueryService,
            [FromServices]
            IGetService<AllPaymentMethodsQuery, IDictionary<Guid, PaymentMethodInfo>> getAllPaymentMethodService,
            [FromServices] IGetService<GetPaymentQuery, PaymentInfo> getPaymentService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var paymentMethods = await getAllPaymentMethodService
                .GetAsync(new())
                .ConfigureAwait(false);

            var query = new BrandPaymentsQuery(brandId, playerTokenData.PlayerCode);

            var brandPayments = await brandPaymentsQueryService.QueryAsync(query)
                .ToArrayAsync()
                .ConfigureAwait(false);

            foreach (var bp in brandPayments)
            {
                if (bp.TransferType != TransferType.Withdraw)
                    continue;

                var getPaymentQuery = new GetPaymentQuery(bp.PaymentId);

                var paymentInfo = await getPaymentService
                    .GetAsync(getPaymentQuery)
                    .ConfigureAwait(false);

                if (paymentInfo.Code == "Manual")
                    continue;

                var paymentMethod = paymentMethods[bp.MethodId];

                yield return new()
                {
                    Id = bp.Id,
                    ChannelId = bp.ChannelId,
                    ChannelName = bp.ChannelName,
                    ChannelCode = bp.ChannelCode,
                    TransferLimit = new()
                    {
                        From = bp.TransferLimit.From,
                        To = bp.TransferLimit.To
                    },
                    ExtendedData = JsonDocument.Parse("{}"),
                    PaymentId = bp.PaymentId,
                    PaymentCode = paymentInfo.Code,
                    MethodId = bp.MethodId,
                    MethodCode = paymentMethod.Name,
                    BankType = paymentMethod.BankType
                };
            }
        }

        /// <summary>
        /// Get the player withdraw info list.
        /// </summary>
        /// <param name="getWithdrawFormsQueryService"></param>
        /// <param name="request"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet("player/list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagedResponse<WithdrawFormViewModel>> GetWithdrawFormsAsync(
            [FromServices] IGetService<GetWithdrawFormsQuery, Tgs.Payment.Clients.PagingResponse<WithdrawFormInfo>> getWithdrawFormsQueryService,
            [FromQuery] GetWithdrawFormsRequest request,
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

            var query = new GetWithdrawFormsQuery(
                brandId,
                playerTokenData.PlayerCode,
                request.Status.ToPaymentFlowState(),
                createOnRange,
                updateOnRange,
                paging);

            var result = await getWithdrawFormsQueryService.GetAsync(query).ConfigureAwait(false);

            var response = new PagedResponse<WithdrawFormViewModel>
            {
                Valid = true,
                TotalCount = result.TotalCount,
                Items = result.Items
                    .Select(form => new WithdrawFormViewModel
                    {
                        Id = form.Id,
                        Status = form.Status.ToFlowState(),
                        Amount = form.Amount,
                        BrandPaymentId = form.BrandPaymentId,
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
        public Task<Dictionary<int, string>> GetWithdrawFormStatusAsync()
        {
            var dict = Enum.GetValues(typeof(FlowState))
               .Cast<FlowState>()
               .Where(x => x != FlowState.Unknown)
               .ToDictionary(t => (int)t, t => t.ToString());
            return Task.FromResult(dict);
        }
    }
}