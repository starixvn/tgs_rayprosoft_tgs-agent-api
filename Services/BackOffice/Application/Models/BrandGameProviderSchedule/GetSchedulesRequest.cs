using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class GetSchedulesRequest : PageableRequest, IRequest<PagedResponse<ScheduleModel>>
    {
        public string Keyword { get; set; }
    }
}