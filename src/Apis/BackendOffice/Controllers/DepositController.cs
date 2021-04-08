using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;
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
        public IAsyncEnumerable<DepositFormInfo> QueryDepositForms(
            [FromServices] IQueryService<DepositFormQuery, DepositFormInfo> queryDepositFormsService,
            Guid brandId,
            [FromQuery] DepositFormRequest request)
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
                updateOnRange);

            return queryDepositFormsService.QueryAsync(query);
        }
    }
}
