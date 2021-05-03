using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
using TGS.Apis.BackendOffice.Controllers.Messages.Withdraw;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Withdraw;
using TGS.Services.BackOffice.Application.Queries.Withdraw;
using TGS.Services.BackOffice.Domain;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : BaseContextController
    {
        [HttpGet("{brandId}/{formId}")]
        public async Task<GenericResponse<WithdrawFormDetailViewModel>> GetWithdrawFormDetailAsync(
            [FromServices] IGetService<GetWithdrawFormDetailQuery, WithdrawFormInfo> getDepositFormService,
            Guid brandId,
            Guid formId)
        {
            var formDetail = await getDepositFormService
                .GetAsync(new GetWithdrawFormDetailQuery(formId, brandId))
                .ConfigureAwait(false);

            return new GenericResponse<WithdrawFormDetailViewModel>
            {
                Valid = true,
                Data = new WithdrawFormDetailViewModel
                {
                    Id = formDetail.Id,
                    PlayerCode = formDetail.PlayerCode,
                    UserName = formDetail.UserName,
                    BrandId = formDetail.BrandId,
                    BrandPaymentId = formDetail.BrandPaymentId,
                    PaymentMethodId = formDetail.PaymentMethodId,
                    ChannelName = formDetail.ChannelName,
                    Amount = formDetail.Amount,
                    Remark = formDetail.Remark,
                    Status = formDetail.Status switch
                    {
                        7000 => FlowStatus.Rejected,
                        8000 => FlowStatus.Approved,
                        _ => FlowStatus.Review
                    },
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
        public async Task<PagedResponse<WithdrawFormViewModel>> QueryDepositFormsAsync(
            [FromServices] IQueryService<WithdrawFormQuery, WithdrawFormModel> queryDepositFormsService,
            [FromServices] IGetService<WithdrawFormQuery, int> countDepositFormsService,
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
                request.Status switch
                {
                    FlowStatus.Review => new[] { 0, 1000 },
                    FlowStatus.Approved => new[] { 8000 },
                    FlowStatus.Rejected => new[] { 7000 },
                    _ => null
                },
                createOnRange,
                updateOnRange,
                paging);

            var totalCount = await countDepositFormsService.GetAsync(query).ConfigureAwait(false);

            var response = new PagedResponse<WithdrawFormViewModel>
            {
                Valid = true,
                TotalCount = totalCount,
                Items = await queryDepositFormsService.QueryAsync(query)
                    .Select(form => new WithdrawFormViewModel
                    {
                        Id = form.Id,
                        PlayerCode = form.PlayerCode,
                        UserName = form.UserName,
                        BrandId = form.BrandId,
                        BrandPaymentId = form.BrandPaymentId,
                        PaymentMethodId = form.PaymentMethodId,
                        ChannelName = form.ChannelName,
                        Status = form.Status switch
                        {
                            7000 => FlowStatus.Rejected,
                            8000 => FlowStatus.Approved,
                            _ => FlowStatus.Review
                        },
                        Amount = form.Amount,
                        Remark = form.Remark,
                        CreatedOn = form.CreatedOn,
                        UpdatedOn = form.UpdatedOn
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };

            return response;
        }

        [HttpPost("{formId}")]
        public async Task<MessageResponse> ReviewWithdrawFormAsync(
            [FromServices] ICommandService<ReviewWithdrawFormCommand> reviewCommandService,
            Guid formId,
            [FromBody] ReviewRequest reviewResponse)
        {
            var command = new ReviewWithdrawFormCommand(
                formId,
                reviewResponse.ReviewResponse,
                reviewResponse.Remark);

            await reviewCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
