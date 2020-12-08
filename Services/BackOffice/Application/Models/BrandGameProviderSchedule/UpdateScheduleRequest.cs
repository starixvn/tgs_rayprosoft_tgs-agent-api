using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderScheduled
{
    public class UpdateScheduleRequest : ScheduleRequest, IRequest<MessageResponse>
    {
    }
}