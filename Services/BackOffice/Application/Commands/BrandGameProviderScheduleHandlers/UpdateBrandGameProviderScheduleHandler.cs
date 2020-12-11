using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.BrandGameProviderScheduleHandlers
{
    public class UpdateBrandGameProviderScheduleHandler : Handler<UpdateScheduleRequest, MessageResponse>
    {
        private readonly IBrandGameProviderScheduleService _service;

        public UpdateBrandGameProviderScheduleHandler(IBrandGameProviderScheduleService service,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<MessageResponse> Handle(UpdateScheduleRequest request,
            CancellationToken cancellationToken)
        {
            var schedule = await _service.GetBrandGameProviderScheduleAsync(q => q.Id == request.Id);

            if (schedule == null)
            {
                return CreateInvalidResponse<MessageResponse>(BrandGameProviderSchedule.NotFound);
            }

            schedule.From = request.From;
            schedule.To = request.To;
            schedule.DayOfWeek = request.DayOfWeek;

            await _service.UpdateBrandGameProviderScheduleAsync(schedule);

            return CreateValidResponse<MessageResponse>();
        }
    }
}