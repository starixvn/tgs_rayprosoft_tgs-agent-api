using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandAgentDividend;
using TGS.Services.BackOffice.Application.Models.BrandAllAgent;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandAgentDividendConditionController : BaseContextController
    {
        private readonly IMediator _mediator;

        public BrandAgentDividendConditionController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{id}")]
        [Description("Get a Brand")]
        public Task<GetBrandAllAgentResponse> GetBrandAsync(Guid id)
            => _mediator.Send(new GetBrandAllAgentRequest(id));

        [HttpPut]
        [Description("Update the brand agent dividend conditions")]
        public Task<MessageResponse> UpdateBrandAgentDividendConditionsAsync(
            UpdateBrandAgentDividendConditionRequest request)
            => _mediator.Send(request);
    }
}