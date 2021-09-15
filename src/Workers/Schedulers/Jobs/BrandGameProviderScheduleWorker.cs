using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Hangfire.Services;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Workers.Schedulers.Jobs
{
    [Queue(Consts.Queue.Schedule)]
    public sealed class BrandGameProviderScheduleWorker : BaseWork<TgsResponse>
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly ITgsLog _log;
        private readonly IBrandGameProviderService _service;

        public BrandGameProviderScheduleWorker(
            IRedisCacheManager cacheManager,
            IBrandGameProviderService service,
            ITgsLog log)
        {
            _service = service;
            _log = log.SetLogger(nameof(BrandGameProviderScheduleWorker));
            _cacheManager = cacheManager;
        }

        public override async Task<TgsResponse> ExecuteAsync(object context)
        {
            var offset = new TimeSpan(8, 0, 0);
            var dayOfWeek = DateTimeOffset.Now.ToOffset(offset).DayOfWeek;
            var utcNow = DateTimeOffset.Now.ToOffset(offset).DateTime;

            bool Predicate(BrandGameProviderSchedule s)
            {
                var src = DateTimeOffset.Now.ToOffset(offset).DateTime;
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
                    await GetBrandGameProviders(Status.Active, Status.Maintenance, Predicate);

                var brandGameProvidersToBeActivated =
                    await GetBrandGameProviders(Status.Maintenance, Status.Active, Predicate);

                var brandGameProviders = brandGameProvidersToBeMaintenance
                    .Union(brandGameProvidersToBeActivated).ToArray();

                var cacheKeys = brandGameProviders
                    .Select(x => new CacheKey
                    {
                        CacheType = RedisCacheType.BrandGameProviders,
                        Key = x.BrandId
                    }).Distinct().ToArray();

                await _service.UpdateBrandGameProvidersAsync(brandGameProviders);

                await _cacheManager.ClearCacheByBatchAsync(cacheKeys);

                return new();
            }
            catch (Exception ex)
            {
                _log.LogError("Fail to update brand game provider status", ex);

                return new()
                {
                    Error = (int)TgsApiError.SystemError,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<BrandGameProvider[]> GetBrandGameProviders(Status sourceStatus, Status targetStatus,
            Func<BrandGameProviderSchedule, bool> predicate)
        {
            var sourceBrandGameProviders = await _service.GetBrandGameProviderQueries(tracking: true)
                .Where(x => x.Status == sourceStatus && !x.IsManualChangeStatus)
                .ToArrayAsync();

            var brandGameProvidersToUpdate = sourceBrandGameProviders
                .Where(x => !x.Schedules.Any(predicate) && targetStatus == Status.Active ||
                    x.Schedules.Any(predicate) && targetStatus == Status.Maintenance).ToArray();

            foreach (var brandGameProvider in brandGameProvidersToUpdate)
            {
                brandGameProvider.Status = targetStatus;
            }

            return brandGameProvidersToUpdate;
        }
    }
}