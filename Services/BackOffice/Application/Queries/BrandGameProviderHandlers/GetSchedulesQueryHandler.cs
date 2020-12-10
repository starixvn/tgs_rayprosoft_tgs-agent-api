using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.DataService;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameProviderHandlers
{
    public class
        GetSchedulesQueryHandler : Handler<GetSchedulesRequest,
            PagedResponse<ScheduleModel>>
    {
        private readonly IBrandGameProviderScheduleService _service;

        public GetSchedulesQueryHandler(IBrandGameProviderScheduleService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<ScheduleModel>> Handle(
            GetSchedulesRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetBrandGameProviderSchedulesAsync()
                .Include(x => x.BrandGameProvider)
                .ThenInclude(x => x.GameProvider)
                .Where(x => x.IsDefault)
                .Where(request.Keyword, x => x.BrandGameProvider.GameProvider.Name.Contains(request.Keyword))
                .Select(x => new ScheduleModel
                {
                    Id = x.Id,
                    BrandGameProviderId = x.BrandGameProviderId,
                    GameProviderName = x.BrandGameProvider.GameProvider.Name,
                    From = x.From,
                    To = x.To,
                    DayOfWeek = x.DayOfWeek
                }).CreatePagedResponseAsync(request);
        }
    }
}