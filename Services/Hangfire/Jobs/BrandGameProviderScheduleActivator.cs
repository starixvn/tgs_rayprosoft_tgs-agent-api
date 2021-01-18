using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.Hangfire.Jobs
{
    public class BrandGameProviderScheduleActivator : JobActivator
    {
        private readonly ITgsRepository _repository;
        private readonly IBrandGameProviderService _service;

        public BrandGameProviderScheduleActivator(ITgsRepository repository, IBrandGameProviderService service)
        {
            _service = service;
            _repository = repository;
        }

        //TODO: Clean brand game provider catch
        public async Task ChangeBrandGameProviderStatusAsync()
        {
            var dayOfWeek = DateTimeOffset.UtcNow.DayOfWeek;
            var utcNow = DateTimeOffset.UtcNow.UtcDateTime;

            bool Predicate(BrandGameProviderSchedule s)
            {
                var src = DateTimeOffset.UtcNow.UtcDateTime;
                var fromHM = new DateTime(src.Year, src.Month, src.Day, s.From.UtcDateTime.Hour,
                    s.From.UtcDateTime.Minute, 0);
                var toHM = new DateTime(src.Year, src.Month, src.Day, s.To.UtcDateTime.Hour, s.To.UtcDateTime.Minute,
                    0);

                return s.IsDefault && dayOfWeek == s.DayOfWeek && fromHM <= utcNow && toHM >= utcNow || !s.IsDefault &&
                    s.From.UtcDateTime <= DateTimeOffset.UtcNow.UtcDateTime &&
                    s.To.UtcDateTime >= DateTimeOffset.UtcNow.UtcDateTime;
            }

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            var activeBrandGameProviders = await _service.GetBrandGameProvidersAsync(tracking: true)
                .Where(x => x.Schedules.Any(s => s.Status == SwitchStatus.Enabled) && x.Status == Status.Active)
                .ToArrayAsync();
            var doMaintenanceBrandGameProviders = activeBrandGameProviders
                .Where(x => x.Schedules.Any(Predicate)).ToArray();

            var maintenanceBrandGameProviders = await _service.GetBrandGameProvidersAsync(tracking: true)
                .Where(x => x.Schedules.Any(s => s.Status == SwitchStatus.Enabled) && x.Status == Status.Maintenance)
                .ToArrayAsync();
            var doActiveBrandGameProviders = maintenanceBrandGameProviders
                .Where(x => !x.Schedules.Any(Predicate)).ToArray();

            foreach (var brandGameProvider in doMaintenanceBrandGameProviders)
                brandGameProvider.Status = Status.Maintenance;
            foreach (var brandGameProvider in doActiveBrandGameProviders) brandGameProvider.Status = Status.Active;

            await _service.UpdateBrandGameProvidersAsync(doActiveBrandGameProviders);
            await _service.UpdateBrandGameProvidersAsync(doMaintenanceBrandGameProviders);

            await tran.CommitAsync();
        }
    }
}