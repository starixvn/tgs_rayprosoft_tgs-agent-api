using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations
{
    public interface IBrandOperations
    {
        Task<CachedBrand> GetCachedBrandAsync(Guid brandId);

        Task<string> GetBrandCodeByBrandIdAsync(Guid brandId);

        Task<Status?> GetBrandStatusByBrandIdAsync(Guid brandId);

        Task<CachedBrandGameProviderConfiguration> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);

        Task<Guid?> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);
    }

    public sealed class BrandOperations : IBrandOperations
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly Func<IBrandQueries> _brandQueriesFactory;
        private readonly IBrandOperations _interface;

        public BrandOperations(
            IRedisCacheManager cacheManager,
            Func<IBrandQueries> brandQueriesFactory)
        {
            _cacheManager = cacheManager;
            _brandQueriesFactory = brandQueriesFactory;
            _interface = this;
        }

        async Task<CachedBrand> IBrandOperations.GetCachedBrandAsync(Guid brandId)
        {
            var cachedBrand = await GetCachedBrandOrNullAsync(brandId);
            if (cachedBrand != null)
            {
                return cachedBrand;
            }

            var r = await _brandQueriesFactory().GetCachedBrandAsync(brandId);
            if (r == null)
            {
                throw new InvalidOperationException($"Brand not found with id: {brandId}");
            }

            await CacheBrandAsync(brandId, r);

            return new CachedBrand
            {
                Code = r.Code,
                TimeZoneId = r.TimeZoneId,
                Status = r.Status,
                ResellerId = r.ResellerId,
                ResellerCode = r.ResellerCode,
                Urls = new BrandUrls
                {
                    Terms = r.TermsUrl,
                    Cashier = r.CashierUrl,
                    Help = r.HelpUrl,
                    Login = r.LoginUrl,
                    MobileTerms = r.MobileTermsUrl,
                    MobileCashier = r.MobileCashierUrl,
                    MobileHelp = r.MobileHelpUrl,
                    MobileLogin = r.MobileLoginUrl
                }
            };
        }

        async Task<string> IBrandOperations.GetBrandCodeByBrandIdAsync(Guid brandId)
        {
            var value = await _cacheManager.GetHashSetAsync(RedisCacheType.Brand, brandId, CachedBrandFields.BrandCode);
            if (value.HasValue)
            {
                return value;
            }

            var cachedBrand = await _interface.GetCachedBrandAsync(brandId);
            return cachedBrand?.Code;
        }

        async Task<Status?> IBrandOperations.GetBrandStatusByBrandIdAsync(Guid brandId)
        {
            var value = await _cacheManager.GetHashSetAsync(RedisCacheType.Brand, brandId, CachedBrandFields.Status);
            if (value.HasValue)
            {
                return (Status)(int)value;
            }

            var cachedBrand = await _interface.GetCachedBrandAsync(brandId);
            return cachedBrand?.Status;
        }

        private async Task<CachedBrand> GetCachedBrandOrNullAsync(Guid brandId)
        {
            try
            {
                var values = await _cacheManager.GetHashSetAsync(RedisCacheType.Brand, brandId, new RedisValue[]
                {
                    CachedBrandFields.BrandCode,
                    CachedBrandFields.TimeZoneId,
                    CachedBrandFields.Status,
                    CachedBrandFields.ResellerId,
                    CachedBrandFields.ResellerCode,
                    CachedBrandFields.TermsUrl,
                    CachedBrandFields.CashierUrl,
                    CachedBrandFields.HelpUrl,
                    CachedBrandFields.LoginUrl,
                    CachedBrandFields.MobileTermsUrl,
                    CachedBrandFields.MobileCashierUrl,
                    CachedBrandFields.MobileHelpUrl,
                    CachedBrandFields.MobileLoginUrl,
                });

                if (values.All(a => !a.IsNull))
                {
                    return new CachedBrand
                    {
                        Code = values[0],
                        TimeZoneId = values[1],
                        Status = (Status)(int)values[2],
                        ResellerId = Guid.Parse(values[3]),
                        ResellerCode = values[4],
                        Urls = new BrandUrls
                        {
                            Terms = values[5],
                            Cashier = values[6],
                            Help = values[7],
                            Login = values[8],
                            MobileTerms = values[9],
                            MobileCashier = values[10],
                            MobileHelp = values[11],
                            MobileLogin = values[12]
                        }
                    };
                }
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to get cached player for player code: {playerCode}", e);
            }

            return null;
        }

        private async Task CacheBrandAsync(Guid brandId, GetCachedBrandResult r)
        {
            try
            {
                var fields = new[]
                {
                    new HashEntry(CachedBrandFields.BrandCode, r.Code),
                    new HashEntry(CachedBrandFields.TimeZoneId, r.TimeZoneId.ToString()),
                    new HashEntry(CachedBrandFields.Status, (int)r.Status),
                    new HashEntry(CachedBrandFields.ResellerId, r.ResellerId.ToString()),
                    new HashEntry(CachedBrandFields.ResellerCode,r.ResellerCode),
                    new HashEntry(CachedBrandFields.TermsUrl,r.TermsUrl ?? string.Empty),
                    new HashEntry(CachedBrandFields.CashierUrl,r.CashierUrl ?? string.Empty),
                    new HashEntry(CachedBrandFields.HelpUrl,r.HelpUrl??string.Empty),
                    new HashEntry(CachedBrandFields.LoginUrl,r.LoginUrl ?? string.Empty),
                    new HashEntry(CachedBrandFields.MobileTermsUrl,r.MobileTermsUrl??string.Empty),
                    new HashEntry(CachedBrandFields.MobileCashierUrl,r.MobileCashierUrl??string.Empty),
                    new HashEntry(CachedBrandFields.MobileHelpUrl,r.MobileHelpUrl??string.Empty),
                    new HashEntry(CachedBrandFields.MobileLoginUrl,r.MobileLoginUrl??string.Empty),
                };

                await _cacheManager.CacheHashSetAsync(RedisCacheType.Brand, brandId, fields, TimeSpan.FromHours(2), true);
            }
            catch (Exception e)
            {
                //_log.LogError($"Failed to cache player for playerId: {playerCode}", e);
            }
        }

        Task<Guid?> IBrandOperations.GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        {
            throw new NotImplementedException();
        }

        Task<CachedBrandGameProviderConfiguration> IBrandOperations.GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId)
        {
            throw new NotImplementedException();
        }

        private static class CachedBrandFields
        {
            public const string BrandCode = "brandCode";
            public const string TimeZoneId = "timezoneId";
            public const string Status = "status";
            public const string ResellerId = "resellerId";
            public const string ResellerCode = "resellerCode";
            public const string LoginUrl = "loginUrl";
            public const string CashierUrl = "cashierUrl";
            public const string HelpUrl = "helpUrl";
            public const string TermsUrl = "termsUrl";
            public const string MobileLoginUrl = "mobileLoginUrl";
            public const string MobileCashierUrl = "mobileCashierUrl";
            public const string MobileHelpUrl = "mobileHelpUrl";
            public const string MobileTermsUrl = "mobileTermsUrl";
        }
    }
}