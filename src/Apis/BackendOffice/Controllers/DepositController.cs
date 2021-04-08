using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Queries.Deposit;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        [AllowAnonymous]
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
    }
}
