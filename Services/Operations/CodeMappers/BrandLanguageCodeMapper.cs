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
    public interface IBrandLanguageCodeMapper : ICodeMapper
    {
    }

    public sealed class BrandLanguageCodeMapper : BaseCodeMapper, IBrandLanguageCodeMapper
    {
        private readonly IBrandQueries _brandQueries;

        public BrandLanguageCodeMapper(
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
                RedisCacheType.Languages,
                Consts.CachePrefix.BrandToTgsMap,
                "language",
                30,
                id => _brandQueries.GetBrandLanguagesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{externalCode}' for '{Consts.CachePrefix.BrandToTgsMap}' used for 'language' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }

        async Task<string> ICodeMapper.InternalToExternalAsync(string internalCode, Guid licenseeId)
        {
            var (found, code) = await GetMappedCodeAsync(
                internalCode,
                licenseeId,
                RedisCacheType.Languages,
                Consts.CachePrefix.TgsToBrandMap,
                "language",
                30,
                id => _brandQueries.GetBrandLanguagesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{internalCode}' for '{Consts.CachePrefix.TgsToBrandMap}' used for 'language' with filter id '{licenseeId}' (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }
    }
}