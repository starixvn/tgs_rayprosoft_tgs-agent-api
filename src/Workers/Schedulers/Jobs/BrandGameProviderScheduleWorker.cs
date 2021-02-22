using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs
{
    [Queue(Consts.Queue.Schedule)]
    public sealed class BrandGameProviderScheduleWorker : BaseWork<TgsResponse>
    {
        private readonly ITgsLog _log;
        private readonly ITgsRepository _repository;
        private readonly IBrandGameProviderService _service;
        private readonly ITransactionManager _transactionManager;

        public BrandGameProviderScheduleWorker(
            ITgsRepository repository,
            ITransactionManager transactionManager,
            IBrandGameProviderService service,
            ITgsLog log)
        {
            _service = service;
            _transactionManager = transactionManager;
            _repository = repository;
            _log = log.SetLogger(nameof(BrandGameProviderScheduleWorker));
        }

        protected override async Task<TgsResponse> ExecuteAsync(object context)
        {
            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

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

            try
            {
                var response = await _transactionManager.ExecuteTransactionAsync(() => _repository,
                    IsolationLevel.ReadCommitted, id,
                    async _ =>
                    {
                        var activeBrandGameProviders = await _service.GetBrandGameProvidersAsync(tracking: true)
                            .Where(x => x.Schedules.Any(s => s.Status == SwitchStatus.Enabled) &&
                                x.Status == Status.Active)
                            .ToArrayAsync();

                        var doMaintenanceBrandGameProviders = activeBrandGameProviders
                            .Where(x => x.Schedules.Any(Predicate)).ToArray();

                        var maintenanceBrandGameProviders = await _service.GetBrandGameProvidersAsync(tracking: true)
                            .Where(x => x.Schedules.Any(s => s.Status == SwitchStatus.Enabled) &&
                                x.Status == Status.Maintenance)
                            .ToArrayAsync();

                        var doActiveBrandGameProviders = maintenanceBrandGameProviders
                            .Where(x => !x.Schedules.Any(Predicate)).ToArray();

                        foreach (var brandGameProvider in doMaintenanceBrandGameProviders)
                        {
                            brandGameProvider.Status = Status.Maintenance;
                        }

                        foreach (var brandGameProvider in doActiveBrandGameProviders)
                        {
                            brandGameProvider.Status = Status.Active;
                        }

                        await _service.UpdateBrandGameProvidersAsync(doActiveBrandGameProviders);
                        await _service.UpdateBrandGameProvidersAsync(doMaintenanceBrandGameProviders);

                        return new TgsResponse();
                    });

                return response;
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to update brand game provider status", ex);

                return new()
                {
                    Error = (int) TgsApiError.SystemError,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}