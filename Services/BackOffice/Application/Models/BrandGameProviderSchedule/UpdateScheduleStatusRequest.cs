using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class UpdateScheduleStatusRequest : BaseRequest, IRequest<MessageResponse>
    {
        public UpdateScheduleStatusRequest(Guid scheduleId, ScheduleStatus status)
        {
            ScheduleId = scheduleId;
            Status = status;
        }

        public Guid ScheduleId { get; set; }
        public ScheduleStatus Status { get; set; }
    }
}