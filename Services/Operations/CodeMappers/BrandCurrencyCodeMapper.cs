using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.CodeMapper;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations.CodeMappers
{
    public interface IBrandCurrencyCodeMapper : ICodeMapper
    {
    }

    public sealed class BrandCurrencyCodeMapper : BaseCodeMapper, IBrandCurrencyCodeMapper
    {
        private readonly IBrandQueries _brandQueries;

        public BrandCurrencyCodeMapper(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IBrandQueries brandQueries)
            : base(cacheManager, json)
        {
            _brandQueries = brandQueries;
        }

        async Task<string> ICodeMapper.ExternalToInternalAsync(string externalCode, Guid licenseeId)
        {
            var (found, code) = await GetMappedCodeAsync(
                externalCode,
                licenseeId,
                RedisCacheType.Currencies,
                Consts.CachePrefix.BrandToTgsMap,
                "currency",
                30,
                id => _brandQueries.GetBrandCurrenciesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{externalCode}' for '{Consts.CachePrefix.BrandToTgsMap}' used for 'currency' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Currencies})");
            }

            return code;
        }

        async Task<string> ICodeMapper.InternalToExternalAsync(string internalCode, Guid licenseeId)
        {
            var (found, code) = await GetMappedCodeAsync(
                internalCode,
                licenseeId,
                RedisCacheType.Currencies,
                Consts.CachePrefix.TgsToBrandMap,
                "currency",
                30,
                id => _brandQueries.GetBrandCurrenciesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{internalCode}' for '{Consts.CachePrefix.TgsToBrandMap}' used for 'currency' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Currencies})");
            }

            return code;
        }
    }
}