using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameProviderHandlers
{
    public class
        GetBrandGameProvidersQueryHandler : Handler<GetBrandGameProvidersRequest,
            PagedResponse<BrandGameProviderModel>>
    {
        private readonly IBrandGameProviderService _service;

        public GetBrandGameProvidersQueryHandler(IBrandGameProviderService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<BrandGameProviderModel>> Handle(
            GetBrandGameProvidersRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetBrandGameProvidersAsync()
                .Include(x => x.Schedules)
                .Where(request.Keyword, x => x.GameProvider.Name.Contains(request.Keyword))
                .Select(x => new BrandGameProviderModel
                {
                    Id = x.Id,
                    Status = x.Status,
                    BrandGameProviderId = x.Id,
                    GameProviderName = x.GameProvider.Name,
                    From = x.Schedules.FirstOrDefault(s => s.IsDefault).From,
                    To = x.Schedules.FirstOrDefault(s => s.IsDefault).To,
                    DayOfWeek = x.Schedules.FirstOrDefault(s => s.IsDefault).DayOfWeek,
                    ScheduleStatus = x.Schedules.FirstOrDefault(s => s.IsDefault).Status
                }).CreatePagedResponseAsync(request);
        }
    }
}