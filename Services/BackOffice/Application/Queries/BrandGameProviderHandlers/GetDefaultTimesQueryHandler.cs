using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameProviderHandlers
{
    public class
        GetDefaultTimesQueryHandler : Handler<GetDefaultTimesRequest,
            GetDefaultTimesResponse>
    {
        private readonly IBrandGameProviderScheduleService _service;

        public GetDefaultTimesQueryHandler(IBrandGameProviderScheduleService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<GetDefaultTimesResponse> Handle(
            GetDefaultTimesRequest request,
            CancellationToken cancellationToken)
        {
            var defaultTimes = await _service.GetBrandGameProviderSchedulesAsync()
                .Where(x => x.BrandGameProviderId == request.BrandGameProviderId)
                .Where(x => !x.IsDefault)
                .Select(x => new DefaultTimeModel
                {
                    From = x.From,
                    To = x.To,
                    IsExpired = DateTimeOffset.Compare(x.To, DateTimeOffset.UtcNow) < 0
                }).ToArrayAsync(cancellationToken);

            return new GetDefaultTimesResponse
            {
                Valid = true,
                DefaultTimes = defaultTimes
            };
        }
    }
}