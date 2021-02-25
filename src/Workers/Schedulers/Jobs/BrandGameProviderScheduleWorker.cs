using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs
{
    [Queue(Consts.Queue.Schedule)]
    public sealed class BrandGameProviderScheduleWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsLog _log;
        private readonly IBrandGameProviderService _service;

        public BrandGameProviderScheduleWorker(
            IBrandGameProviderService service,
            ITgsLog log)
        {
            _service = service;
            _log = log.SetLogger(nameof(BrandGameProviderScheduleWorker));
        }

        protected override async Task<TgsResponse> ExecuteAsync(object context)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            var dayOfWeek = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).DayOfWeek;
            var utcNow = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).DateTime;

            bool Predicate(BrandGameProviderSchedule s)
            {
                var src = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).DateTime;
                var fromHM = new DateTime(src.Year, src.Month, src.Day, s.From.Hour,
                    s.From.Minute, 0);
                var toHM = new DateTime(src.Year, src.Month, src.Day, s.To.Hour, s.To.Minute,
                    0);

                return s.Status == SwitchStatus.Enabled && s.IsDefault && dayOfWeek == s.DayOfWeek &&
                    fromHM <= utcNow && toHM >= utcNow || !s.IsDefault &&
                    s.From <= DateTimeOffset.Now &&
                    s.To >= DateTimeOffset.Now;
            }

            try
            {
                var brandGameProvidersToBeMaintenance =
                    await UpdateBrandGameProviderStatus(Status.Active, Status.Maintenance, Predicate);

                var brandGameProvidersToBeActivated =
                    await UpdateBrandGameProviderStatus(Status.Maintenance, Status.Active, Predicate);

                await _service.UpdateBrandGameProvidersAsync(brandGameProvidersToBeMaintenance
                    .Union(brandGameProvidersToBeActivated).ToArray());

                return new TgsResponse();
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to update brand game provider status", ex);

                return new TgsResponse
                {
                    Error = (int) TgsApiError.SystemError,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<BrandGameProvider[]> UpdateBrandGameProviderStatus(Status sourceStatus, Status targetStatus,
            Func<BrandGameProviderSchedule, bool> Predicate)
        {
            var sourceBrandGameProviders = await _service.GetBrandGameProvidersAsync(tracking: true)
                .Where(x => x.Status == sourceStatus && !x.IsManualChangeStatus)
                .ToArrayAsync();

            var brandGameProvidersToUpdate = sourceBrandGameProviders
                .Where(x => !x.Schedules.Any(Predicate) && targetStatus == Status.Active ||
                            x.Schedules.Any(Predicate) && targetStatus == Status.Maintenance).ToArray();

            foreach (var brandGameProvider in brandGameProvidersToUpdate)
                brandGameProvider.Status = targetStatus;

            return brandGameProvidersToUpdate;
        }
    }
}