using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Deposit;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : BaseContextController
    {
        [HttpGet("exportDeposit/{brandId}")]
        public async Task<IActionResult> ExportDepositAsync(
            [FromServices] IGetService<ExportDepositForm, MemoryStream> queryService,
            Guid brandId,
            [FromQuery] DepositFormRequest request,
            [FromQuery] PageableRequest paging)
        {
            var createOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;
            var updateOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;

            if (request.BeginCreateOn != null && request.EndCreateOn != null)
                createOnRange = (request.BeginCreateOn.Value, request.EndCreateOn.Value);

            if (request.BeginUpdateOn != null && request.EndUpdateOn != null)
                updateOnRange = (request.BeginUpdateOn.Value, request.EndUpdateOn.Value);

            var query = new ExportDepositForm(
                brandId,
                request.UserName,
                request.ChannelName,
                request.ThirdPartyOrderNo,
                request.FormId,
                request.PaymentMethodId,
                request.Status.SelectMany(s => s.ToPaymentFlowState()).Distinct(),
                createOnRange,
                updateOnRange,
                paging);

            return File(
                await queryService.GetAsync(query).ConfigureAwait(false),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Deposit.xlsx");
        }

        /// <summary>
        /// Gets the deposit form detail.
        /// </summary>
        /// <param name="getDepositFormService">The get deposit form service.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="formId">The form identifier.</param>
        /// <returns></returns>
        [HttpGet("{brandId}/{formId}")]
        public async Task<GenericResponse<DepositFormDetailViewModel>> GetDepositFormDetailAsync(
            [FromServices] IGetService<GetDepositFormDetailQuery, DepositFormInfo> getDepositFormService,
            Guid brandId,
            Guid formId)
        {
            var formDetail = await getDepositFormService
                .GetAsync(new GetDepositFormDetailQuery(formId, brandId))
                .ConfigureAwait(false);

            return new GenericResponse<DepositFormDetailViewModel>
            {
                Valid = true,
                Data = new DepositFormDetailViewModel
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
                    Status = formDetail.Status.ToFlowState(),
                    ApplyInfo = string.IsNullOrWhiteSpace(formDetail.ApplyInfo)
                        ? null
                        : JsonDocument.Parse(formDetail.ApplyInfo),
                    PaymentReturn = string.IsNullOrWhiteSpace(formDetail.PaymentReturn)
                        ? null
                        : JsonDocument.Parse(formDetail.PaymentReturn),
                    CreatedOn = formDetail.CreatedOn,
                    UpdatedOn = formDetail.UpdatedOn,
                    ThirdPartyOrderNo = formDetail.ThirdPartyOrderNo
                }
            };
        }

        /// <summary>
        /// Queries the deposit forms.
        /// </summary>
        /// <param name="queryService">The query service.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        [HttpGet("{brandId}")]
        public async Task<DepositViewModel> QueryDepositFormsAsync(
            [FromServices] IGetService<DepositFormQuery, DepositForms> queryService,
            Guid brandId,
            [FromQuery] DepositFormRequest request,
            [FromQuery] PageableRequest paging)
        {
            var createOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;
            var updateOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;

            if (request.BeginCreateOn != null && request.EndCreateOn != null)
                createOnRange = (request.BeginCreateOn.Value, request.EndCreateOn.Value);

            if (request.BeginUpdateOn != null && request.EndUpdateOn != null)
                updateOnRange = (request.BeginUpdateOn.Value, request.EndUpdateOn.Value);

            var query = new DepositFormQuery(
                brandId,
                request.UserName,
                request.ChannelName,
                request.ThirdPartyOrderNo,
                request.FormId,
                request.PaymentMethodId,
                request.Status.SelectMany(s => s.ToPaymentFlowState()).Distinct(),
                createOnRange,
                updateOnRange,
                paging);

            var result = await queryService.GetAsync(query).ConfigureAwait(false);

            var response = new DepositViewModel
            {
                Valid = true,
                TotalCount = result.TotalCount,
                Items = result.Items
                    .Select(form => new DepositFormViewModel
                    {
                        Id = form.Id,
                        PlayerCode = form.PlayerCode,
                        UserName = form.UserName,
                        BrandId = form.BrandId,
                        BrandPaymentId = form.BrandPaymentId,
                        PaymentMethodId = form.PaymentMethodId,
                        ChannelName = form.ChannelName,
                        Status = form.Status.ToFlowState(),
                        Amount = form.Amount,
                        Remark = form.Remark,
                        CreatedOn = form.CreatedOn,
                        UpdatedOn = form.UpdatedOn,
                        ThirdPartyOrderNo = form.ThirdPartyOrderNo,
                        ReviewedUserName = form.ReviewedUserName
                    })
                    .ToArray(),
                Sum = result.Sum
            };

            return response;
        }

        /// <summary>
        /// Reviews the deposit form.
        /// </summary>
        /// <param name="reviewCommandService">The review command service.</param>
        /// <param name="formId">The form identifier.</param>
        /// <param name="reviewResponse">The review responses.</param>
        [HttpPost("{formId}")]
        public async Task<MessageResponse> ReviewDepositFormAsync(
            [FromServices] ICommandService<ReviewDepositFormCommand> reviewCommandService,
            Guid formId,
            [FromBody] DepositReviewRequest reviewResponse)
        {
            var command = new ReviewDepositFormCommand(
                reviewResponse.BrandId,
                formId,
                reviewResponse.ReviewResponse.ToReviewState(),
                null,
                reviewResponse.Remark,
                UserId,
                Username);

            await reviewCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
