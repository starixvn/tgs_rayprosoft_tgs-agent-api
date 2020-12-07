using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProvider
{
    public class SetScheduledMaintenanceTimeRequest : IRequest<MessageResponse>
    {
        public MaintenanceTime[] MaintenanceTimes { get; set; }
    }

    public class MaintenanceTime
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}