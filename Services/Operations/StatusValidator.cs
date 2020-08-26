using System;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Infrastructures.Redis;

namespace TGS.Services.Operations
{
    public interface IStatusValidator
    {
        Task ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] status);

        Task ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderConfigurationId, params Status[] status);

    }

    public sealed class StatusValidator : IStatusValidator
    {
        private static readonly TimeSpan CacheFor = TimeSpan.MaxValue;

        private readonly IRedisCacheManager _cacheManager;

        public StatusValidator(IRedisCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        async Task IStatusValidator.ThrowIfBrandStatusIsNotOneOfAsync(Guid brandId, params Status[] status)
        {
            EnsureStatus(status);

            //var cachedBrand = await _cacheManager.SafeGetAndCacheAsync(
            //    RedisCacheType.Brand,
            //    brandId,
            //    CacheFor,
            //    () =>
            //    {
            //        var bq = _brandQueriesFactory();
            //        return bq.GetBrandAndLicenseeStatusesByIdAsync(brandId);
            //    });

            //if (currentStatus == null)
            //{
            //    throw new InvalidStatusException($"{Const.Brand} doesn't exist");
            //}

            //if (!status.Contains(UgsStatus.Maintenance) && currentStatus.BrandStatus == UgsStatus.Maintenance)
            //{
            //    throw new BrandUnderMaintenanceException();
            //}

            //if (!status.Contains(currentStatus.BrandStatus) || !status.Contains(currentStatus.LicenseeStatus))
            //{
            //    throw new InvalidStatusException($"{Const.Brand} doesn't exist or is not active");
            //}
        }

        Task IStatusValidator.ThrowIfGameProviderOrConfigurationStatusIsNotOneOfAsync(Guid gameProviderConfigurationId, params Status[] status)
        {
            throw new NotImplementedException();
        }

        private static void EnsureStatus(Status[] status)
        {
            if (status == null || status.Length == 0)
                throw new ArgumentException("No status provided");
        }
    }
}