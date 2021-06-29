using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Withdraw;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Withdraw;
using TGS.Services.BackOffice.Application.Queries.Withdraw;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : BaseContextController
    {
        [HttpGet("{brandId}/{formId}")]
        public async Task<GenericResponse<WithdrawFormDetailViewModel>> GetWithdrawFormDetailAsync(
            [FromServices] IGetService<GetWithdrawFormDetailQuery, WithdrawFormInfo> getWithdrawFormService,
            Guid brandId,
            Guid formId)
        {
            var formDetail = await getWithdrawFormService
                .GetAsync(new GetWithdrawFormDetailQuery(formId, brandId))
                .ConfigureAwait(false);

            return new GenericResponse<WithdrawFormDetailViewModel>
            {
                Valid = true,
                Data = new WithdrawFormDetailViewModel
                {
                    Id = formDetail.Id,
                    UserId = formDetail.UserId,
                    UserName = formDetail.UserName,
                    UserType = formDetail.UserType.ToString(),
                    BrandId = formDetail.BrandId,
                    BrandPaymentId = formDetail.BrandPaymentId,
                    PaymentMethodId = formDetail.PaymentMethodId,
                    ChannelName = formDetail.ChannelName,
                    Amount = formDetail.Amount,
                    Remark = formDetail.Remark,
                    Status = formDetail.Status.ToFlowState(),
                    BankInfo = string.IsNullOrWhiteSpace(formDetail.BankInfo)
                        ? null
                        : JsonDocument.Parse(formDetail.BankInfo),
                    ApplyInfo = string.IsNullOrWhiteSpace(formDetail.ApplyInfo)
                        ? null
                        : JsonDocument.Parse(formDetail.ApplyInfo),
                    PaymentReturn = string.IsNullOrWhiteSpace(formDetail.PaymentReturn)
                        ? null
                        : JsonDocument.Parse(formDetail.PaymentReturn),
                    CreatedOn = formDetail.CreatedOn,
                    UpdatedOn = formDetail.UpdatedOn
                }
            };
        }

        [HttpGet("{brandId}")]
        public async Task<PagedResponse<WithdrawFormViewModel>> QueryWithdrawFormsAsync(
            [FromServices] IGetService<WithdrawFormQuery, PagingResponse<WithdrawFormInfo>> queryService,
            Guid brandId,
            [FromQuery] WithdrawFormRequest request,
            [FromQuery] PageableRequest paging)
        {
            var createOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;
            var updateOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;

            if (request.BeginCreateOn != null && request.EndCreateOn != null)
                createOnRange = (request.BeginCreateOn.Value, request.EndCreateOn.Value);

            if (request.BeginUpdateOn != null && request.EndUpdateOn != null)
                updateOnRange = (request.BeginUpdateOn.Value, request.EndUpdateOn.Value);

            var query = new WithdrawFormQuery(
                brandId,
                request.UserName,
                request.ChannelName,
                request.PaymentMethodId,
                request.Status.ToPaymentFlowState(),
                createOnRange,
                updateOnRange,
                paging);

            var result = await queryService.GetAsync(query).ConfigureAwait(false);

            var response = new PagedResponse<WithdrawFormViewModel>
            {
                Valid = true,
                TotalCount = result.TotalCount,
                Items = result.Items
                    .Select(form => new WithdrawFormViewModel
                    {
                        Id = form.Id,
                        UserId = form.UserId,
                        UserName = form.UserName,
                        UserType = form.UserType.ToString(),
                        BrandId = form.BrandId,
                        BrandPaymentId = form.BrandPaymentId,
                        PaymentMethodId = form.PaymentMethodId,
                        ChannelName = form.ChannelName,
                        Status = form.Status.ToFlowState(),
                        Amount = form.Amount,
                        Remark = form.Remark,
                        CreatedOn = form.CreatedOn,
                        UpdatedOn = form.UpdatedOn
                    })
                    .ToArray()
            };

            return response;
        }

        [HttpPost("{brandId}/{formId}")]
        public async Task<MessageResponse> ReviewWithdrawFormAsync(
            [FromServices] ICommandService<ReviewWithdrawFormCommand> reviewCommandService,
            Guid brandId,
            Guid formId,
            [FromBody] WithdrawReviewRequest reviewResponse)
        {
            var command = new ReviewWithdrawFormCommand(
                brandId,
                formId,
                reviewResponse.BrandPaymentId,
                reviewResponse.ReviewResponse.ToReviewState(),
                reviewResponse.Remark);

            await reviewCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
