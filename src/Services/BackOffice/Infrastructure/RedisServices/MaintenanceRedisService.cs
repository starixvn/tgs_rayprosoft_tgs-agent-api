using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;

namespace TGS.Services.BackOffice.Infrastructure.RedisServices
{
    public interface IMaintenanceRedisService
    {
        ValueTask AddResellerMaintenancesAsync(Guid[] ids);
        ValueTask AddResellerMaintenanceAsync(Guid id);
        ValueTask<string> GetResellerMaintenanceAsync(Guid ids);
        ValueTask ClearResellerMaintenancesAsync(Guid[] ids);
        ValueTask ClearResellerMaintenanceAsync(Guid id);

        ValueTask AddLicenseeMaintenancesAsync(Guid[] ids);
        ValueTask AddLicenseeMaintenanceAsync(Guid id);
        ValueTask<string> GetLicenseeMaintenanceAsync(Guid id);
        ValueTask CleartLicenseeMaintenancesAsync(Guid[] ids);
        ValueTask CleartLicenseeMaintenanceAsync(Guid id);

        ValueTask AddBrandMaintenancesAsync(Guid[] ids);
        ValueTask AddBrandMaintenanceAsync(Guid id);
        ValueTask<string> GetBrandMaintenanceAsync(Guid id);
        ValueTask CleartBrandMaintenancesAsync(Guid[] ids);
        ValueTask CleartBrandMaintenanceAsync(Guid id);
    }

    public sealed class MaintenanceRedisService : IMaintenanceRedisService
    {
        private const string _resellers = "Resellers";
        private const string _licensees = "Licensees";
        private const string _brands = "Brands";
        private const string _maintenance = "Maintenance";
        private readonly IRedisCacheManager _cacheManager;

        public MaintenanceRedisService(IRedisCacheManager cacheManager)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        async ValueTask IMaintenanceRedisService.AddResellerMaintenancesAsync(Guid[] ids)
        {
            var tasks = new List<Task>();
            foreach (var id in ids)
            {
                var task = AddResellerMaintenanceAsync(id).AsTask();
                tasks.Add(task);

            }

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
        }
        public async ValueTask AddResellerMaintenanceAsync(Guid id)
        {
            await AddMaintenanceAsync(_resellers, id).ConfigureAwait(false);
        }
        async ValueTask<string> IMaintenanceRedisService.GetResellerMaintenanceAsync(Guid id)
        {
            return await GetMaintenanceAsync(_resellers, id).ConfigureAwait(false);
        }
        async ValueTask IMaintenanceRedisService.ClearResellerMaintenancesAsync(Guid[] ids)
        {
            await ClearMaintenancesAsync(_resellers, ids).ConfigureAwait(false);
        }
        async ValueTask IMaintenanceRedisService.ClearResellerMaintenanceAsync(Guid id)
        {
            await ClearMaintenanceAsync(_resellers, id).ConfigureAwait(false);
        }

        async ValueTask IMaintenanceRedisService.AddLicenseeMaintenancesAsync(Guid[] ids)
        {
            var tasks = new List<Task>();
            foreach (var id in ids)
            {
                var task = AddLicenseeMaintenanceAsync(id).AsTask();
                tasks.Add(task);

            }

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
        }
        public async ValueTask AddLicenseeMaintenanceAsync(Guid id)
        {
            await AddMaintenanceAsync(_licensees, id).ConfigureAwait(false);
        }
        async ValueTask<string> IMaintenanceRedisService.GetLicenseeMaintenanceAsync(Guid id)
        {
            return await GetMaintenanceAsync(_licensees, id).ConfigureAwait(false);
        }
        async ValueTask IMaintenanceRedisService.CleartLicenseeMaintenancesAsync(Guid[] ids)
        {
            await ClearMaintenancesAsync(_licensees, ids).ConfigureAwait(false);
        }
        async ValueTask IMaintenanceRedisService.CleartLicenseeMaintenanceAsync(Guid id)
        {
            await ClearMaintenanceAsync(_licensees, id).ConfigureAwait(false);
        }

        async ValueTask IMaintenanceRedisService.AddBrandMaintenancesAsync(Guid[] ids)
        {
            var tasks = new List<Task>();
            foreach (var id in ids)
            {
                var task = AddBrandMaintenanceAsync(id).AsTask();
                tasks.Add(task);

            }
            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
        }
        public async ValueTask AddBrandMaintenanceAsync(Guid id)
        {
            await AddMaintenanceAsync(_brands, id).ConfigureAwait(false);
        }
        async ValueTask<string> IMaintenanceRedisService.GetBrandMaintenanceAsync(Guid id)
        {
            return await GetMaintenanceAsync(_brands, id).ConfigureAwait(false);
        }
        async ValueTask IMaintenanceRedisService.CleartBrandMaintenancesAsync(Guid[] ids)
        {
            await ClearMaintenancesAsync(_brands, ids).ConfigureAwait(false);
        }
        async ValueTask IMaintenanceRedisService.CleartBrandMaintenanceAsync(Guid id)
        {
            await ClearMaintenanceAsync(_brands, id).ConfigureAwait(false);
        }

        private string GetMaintenanceKey(string merchantKey, Guid id)
        {
            return $"{merchantKey }:{id}";
        }

        private Task AddMaintenanceAsync(string merchantKey, Guid id)
        {
            return _cacheManager.SafeCacheForeverAsync(_maintenance, GetMaintenanceKey(merchantKey, id), id);
        }

        private async ValueTask<string> GetMaintenanceAsync(string merchantKey, Guid id)
        {
            return await _cacheManager.SafeGetAsync(_maintenance, GetMaintenanceKey(merchantKey, id), () => Task.FromResult(default(string))).ConfigureAwait(false);
        }

        private async ValueTask ClearMaintenanceAsync(string merchantKey, Guid id)
        {
            await _cacheManager.ClearCacheAsync(_maintenance, GetMaintenanceKey(merchantKey, id)).ConfigureAwait(false);
        }

        private async ValueTask<int> ClearMaintenancesAsync(string merchantKey, Guid[] ids)
        {
            var cacheKeys = (from id in ids.ToList()
                             select new CacheKey()
                             {
                                 CacheType = _maintenance,
                                 Key = GetMaintenanceKey(merchantKey, id)
                             }).ToArray();
            return await _cacheManager.ClearCacheByBatchAsync(cacheKeys).ConfigureAwait(false);
        }
    }
}
