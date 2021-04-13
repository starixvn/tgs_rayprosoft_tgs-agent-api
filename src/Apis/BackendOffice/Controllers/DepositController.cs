using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.Deposit;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Domain.Enums;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        /// <summary>
        /// Queries the deposit forms.
        /// </summary>
        /// <param name="queryDepositFormsService">The query deposit forms service.</param>
        /// <param name="countDepositFormsService">The count deposit forms service.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        [HttpGet("{brandId}")]
        public async Task<PagedResponse<DepositFormViewModel>> QueryDepositFormsAsync(
            [FromServices] IQueryService<DepositFormQuery, DepositFormInfo> queryDepositFormsService,
            [FromServices] IGetService<DepositFormQuery, int> countDepositFormsService,
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
                request.PaymentMethodId,
                request.Status,
                createOnRange,
                updateOnRange,
                paging);

            var response = new PagedResponse<DepositFormViewModel>
            {
                Valid = true,
                TotalCount = await countDepositFormsService.GetAsync(query).ConfigureAwait(false),
                Items = await queryDepositFormsService.QueryAsync(query)
                    .Select(form => new DepositFormViewModel
                    {
                        Id = form.Id,
                        PlayerCode = form.PlayerCode,
                        UserName = form.UserName,
                        BrandId = form.BrandId,
                        BrandPaymentId = form.BrandPaymentId,
                        PaymentMethodId = form.PaymentMethodId,
                        Amount = form.Amount,
                        Remark = form.Remark,
                        CreateOn = form.CreateOn,
                        UpdateOn = form.UpdateOn
                    })
                    .ToArrayAsync()
                    .ConfigureAwait(false)
            };

            return response;
        }

        /// <summary>
        /// Reviews the deposit form.
        /// </summary>
        /// <param name="reviewCommandService">The review command service.</param>
        /// <param name="formId">The form identifier.</param>
        /// <param name="reviewResponses">The review responses.</param>
        [AllowAnonymous]
        [HttpPost("{formId}")]
        public async Task ReviewDepositFormAsync(
            [FromServices] ICommandService<ReviewDepositFormCommand> reviewCommandService,
            Guid formId,
            [FromBody] ReviewStatus reviewResponses)
        {
            var command = new ReviewDepositFormCommand(
                formId,
                reviewResponses);

            await reviewCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }
    }
}
