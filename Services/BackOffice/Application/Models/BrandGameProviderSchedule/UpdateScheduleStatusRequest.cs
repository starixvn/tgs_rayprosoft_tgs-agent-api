using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class UpdateScheduleStatusRequest : BaseRequest, IRequest<MessageResponse>
    {
        public UpdateScheduleStatusRequest(Guid scheduleId, Status status)
        {
            ScheduleId = scheduleId;
            Status = status;
        }

        public Guid ScheduleId { get; set; }
        public Status Status { get; set; }
    }
}