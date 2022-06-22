using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Logging;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Workers.Schedulers.Hangfire.Services;

namespace TGS.Workers.Schedulers.Jobs;

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
        var now = DateTimeOffset.Now.ToOffset(offset);

        bool Predicate(BrandGameProviderSchedule s)
        {
            var src = DateTimeOffset.Now.ToOffset(offset);
            var from = s.From.ToOffset(offset);
            var to = s.To.ToOffset(offset);

            var fromHM = new DateTimeOffset(src.Year,
                src.Month,
                src.Day,
                from.Hour,
                from.Minute,
                0,
                offset);

            var toHM = new DateTimeOffset(src.Year,
                src.Month,
                src.Day,
                to.Hour,
                to.Minute,
                0,
                offset);

            return s.Status == SwitchStatus.Enabled && (s.IsDefault && dayOfWeek == s.DayOfWeek &&
                fromHM <= now && toHM >= now || !s.IsDefault &&
                s.From <= DateTimeOffset.Now &&
                s.To >= DateTimeOffset.Now);
        }

        try
        {
            var brandGameProvidersToBeMaintenanceIds =
                await GetBrandGameProviders(Status.Active, Status.Maintenance, Predicate);

            var brandGameProvidersToBeActivatedIds =
                await GetBrandGameProviders(Status.Maintenance, Status.Active, Predicate);

            var brandGameProviders = brandGameProvidersToBeMaintenanceIds
                .Union(brandGameProvidersToBeActivatedIds).ToArray();

            var cacheKeys = brandGameProviders
                .Select(id => new CacheKey
                {
                    CacheType = RedisCacheType.BrandGameProviders,
                    Key = id
                }).Distinct().ToArray();

            await _service.UpdateBrandGameProvidersAsync(string.Join(",", brandGameProvidersToBeMaintenanceIds),
                string.Join(",", brandGameProvidersToBeActivatedIds));

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

    private async ValueTask<Guid[]> GetBrandGameProviders(
        Status sourceStatus, Status targetStatus,
        Func<BrandGameProviderSchedule, bool> predicate)
    {
        var sourceBrandGameProviders = await _service.GetBrandGameProviderQueries()
            .Where(x => x.Status == sourceStatus && !x.IsManualChangeStatus)
            .Include(x => x.Schedules)
            .ToArrayAsync();

        var brandGameProvidersToUpdate = sourceBrandGameProviders
            .Where(x => !x.Schedules.Any(predicate) && targetStatus == Status.Active ||
                x.Schedules.Any(predicate) && targetStatus == Status.Maintenance).ToArray();

        return brandGameProvidersToUpdate.Select(x => x.Id).ToArray();
    }
}