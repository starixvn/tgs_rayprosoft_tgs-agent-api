using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandGameProviderScheduleHandlers
{
    public class UpdateBrandGameProviderScheduleStatusHandler : Handler<UpdateScheduleStatusRequest, MessageResponse>
    {
        private readonly IBrandGameProviderScheduleService _service;

        public UpdateBrandGameProviderScheduleStatusHandler(IBrandGameProviderScheduleService service,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateScheduleStatusRequest request,
            CancellationToken cancellationToken)
        {
            var schedule = await _service.GetBrandGameProviderScheduleAsync(q => q.Id == request.ScheduleId);

            if (schedule == null)
            {
                return CreateInvalidResponse<MessageResponse>(Brand.NotFound);
            }

            await _service.ChangeStatusAsync(schedule, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}