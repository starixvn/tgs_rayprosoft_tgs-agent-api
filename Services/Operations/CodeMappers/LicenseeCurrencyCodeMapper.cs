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
    public interface ILicenseeCurrencyCodeMapper : ICodeMapper
    {

    }

    public sealed class LicenseeCurrencyCodeMapper : BaseCodeMapper, ILicenseeCurrencyCodeMapper
    {
        private readonly ILicenseeQueries _licenseeQueries;

        public LicenseeCurrencyCodeMapper(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            ILicenseeQueries licenseeQueries) 
            : base(cacheManager, json)
        {
            _licenseeQueries = licenseeQueries;
        }

        async Task<string> ICodeMapper.ExternalToInternalAsync(string externalCode, Guid licenseeId)
        {
            var (found, code) = await GetMappedCodeAsync(
                externalCode,
                licenseeId,
                RedisCacheType.Currencies,
                Consts.CachePrefix.LicenseeToTgsMap,
                "currency",
                30,
                id => _licenseeQueries.GetLicenseeCurrenciesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{externalCode}' for '{Consts.CachePrefix.LicenseeToTgsMap}' used for 'currency' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Currencies})");
            }

            return code;
        }

        async Task<string> ICodeMapper.InternalToExternalAsync(string internalCode, Guid licenseeId)
        {
            var (found, code) = await GetMappedCodeAsync(
                internalCode,
                licenseeId,
                RedisCacheType.Currencies,
                Consts.CachePrefix.TgsToLicenseeMap,
                "currency",
                30,
                id => _licenseeQueries.GetLicenseeCurrenciesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{internalCode}' for '{Consts.CachePrefix.TgsToGameProviderMap}' used for 'currency' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Currencies})");
            }

            return code;
        }
    }
}
