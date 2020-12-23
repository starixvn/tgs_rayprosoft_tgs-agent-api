using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BrandGameProviderScheduleController : BaseContextController
    {
        private readonly IMediator _mediator;

        public BrandGameProviderScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get schedules")]
        public Task<PagedResponse<ScheduleModel>> GetBrandGameProviderSchedulesAsync(
            [FromQuery] GetSchedulesRequest request) =>
            _mediator.Send(request);

        [HttpGet("{brandGameProviderId}/defaultTimes")]
        [Description("Get default times")]
        public Task<GetDefaultTimesResponse> GetBrandGameProviderDefaultTimesAsync(Guid brandGameProviderId) =>
            _mediator.Send(new GetDefaultTimesRequest(brandGameProviderId));

        [HttpPut("{scheduleId}")]
        [Description("Update a schedule")]
        public Task<MessageResponse> UpdateBrandGameProviderScheduleAsync(Guid scheduleId,
            UpdateScheduleRequest request)
        {
            request.Id = scheduleId;
            return _mediator.Send(request);
        }

        [HttpPut("{brandGameProviderId}/defaultTimes")]
        [Description("Update default times")]
        public Task<MessageResponse> UpdateBrandGameProviderDefaultTimesAsync(Guid brandGameProviderId,
            UpdateDefaultTimesRequest request)
        {
            request.BrandGameProviderId = brandGameProviderId;
            return _mediator.Send(request);
        }
    }
}