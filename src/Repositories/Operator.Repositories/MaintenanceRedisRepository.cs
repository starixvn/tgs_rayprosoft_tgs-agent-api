using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Redis;
using TGS.Services.Operator;

namespace TGS.Operator.Repositories
{

    public sealed class MaintenanceRedisRepository : IMaintenanceRedisRepository
    {
        private const string _resellers = "Resellers";
        private const string _licensees = "Licensees";
        private const string _brands = "Brands";
        private const string _maintenance = "Maintenance";
        private readonly IRedisCacheManager _cacheManager;

        public MaintenanceRedisRepository(IRedisCacheManager cacheManager)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        async ValueTask<string> IMaintenanceRedisRepository.GetResellerMaintenanceAsync(Guid id)
        {
            return await GetMaintenanceAsync(_resellers, id).ConfigureAwait(false);
        }

        async ValueTask<string> IMaintenanceRedisRepository.GetLicenseeMaintenanceAsync(Guid id)
        {
            return await GetMaintenanceAsync(_licensees, id).ConfigureAwait(false);
        }

        async ValueTask<string> IMaintenanceRedisRepository.GetBrandMaintenanceAsync(Guid id)
        {
            return await GetMaintenanceAsync(_brands, id).ConfigureAwait(false);
        }

        private async ValueTask<string> GetMaintenanceAsync(string merchantKey, Guid id)
        {
            string key = $"{merchantKey }:{id}";
            return await _cacheManager.SafeGetAsync(_maintenance, key, () => Task.FromResult(default(string))).ConfigureAwait(false);
        }
    }
}
