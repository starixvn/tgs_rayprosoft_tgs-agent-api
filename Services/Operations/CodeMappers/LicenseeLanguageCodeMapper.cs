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
    public interface ILicenseeLanguageCodeMapper : ICodeMapper
    {

    }

    public sealed class LicenseeLanguageCodeMapper : BaseCodeMapper, ILicenseeLanguageCodeMapper
    {
        private readonly ILicenseeQueries _licenseeQueries;

        public LicenseeLanguageCodeMapper(
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
                RedisCacheType.Languages,
                Consts.CachePrefix.LicenseeToTgsMap,
                "language",
                30,
                id => _licenseeQueries.GetLicenseeLanguagesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{externalCode}' for '{Consts.CachePrefix.LicenseeToTgsMap}' used for 'language' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }

        async Task<string> ICodeMapper.InternalToExternalAsync(string internalCode, Guid licenseeId)
        {
            var (found, code) = await GetMappedCodeAsync(
                internalCode,
                licenseeId,
                RedisCacheType.Languages,
                Consts.CachePrefix.TgsToLicenseeMap,
                "language",
                30,
                id => _licenseeQueries.GetLicenseeLanguagesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{internalCode}' for '{Consts.CachePrefix.TgsToLicenseeMap}' used for 'language' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }
    }
}
