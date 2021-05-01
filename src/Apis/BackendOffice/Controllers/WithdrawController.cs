using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
using TGS.Apis.BackendOffice.Controllers.Messages.Withdraw;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.Withdraw;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        [HttpGet("{brandId}")]
        public async Task<PagedResponse<WithdrawFormViewModel>> QueryDepositFormsAsync(
            [FromServices] IQueryService<WithdrawFormQuery, WithdrawFormInfo> queryDepositFormsService,
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
    }
}
