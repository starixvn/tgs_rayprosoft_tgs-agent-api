using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models.Enums;

namespace TGS.Core.Infrastructures.CodeMapper
{
    public interface ICodeMapper
    {
        Task<string> ExternalToInternalAsync(
            string externalCode,
            Guid id);

        Task<string> InternalToExternalAsync(
            string internalCode,
            Guid id);

        Task<Dictionary<string, string>> GetMapByExternalCodeAsync(
            Guid id);

        Task<Dictionary<string, string>> GetMapByInternalCodeAsync(
            Guid id);
    }

    public abstract class BaseCodeMapper
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IJsonSerializationProvider _json;

        protected BaseCodeMapper(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json)
        {
            _cacheManager = cacheManager;
            _json = json;
        }

        protected async Task<(bool Found, string Code)> GetMappedCodeAsync(
            string lookUpCode,
            Guid filterId,
            RedisCacheType cacheType,
            string cacheKeyPrefix,
            string usedFor,
            int cacheForMinutes,
            Func<Guid, Task<Dictionary<string, string>>> getDictionary)
        {
            if (string.IsNullOrWhiteSpace(lookUpCode))
            {
                throw new ArgumentException(
                    $"Missing key '{lookUpCode}' for \'{cacheKeyPrefix}\' used for \'{usedFor}\' with filter id {filterId} (intended to be cached as {cacheType})");
            }

            var dict = await GetMappedCodesAsync(filterId, cacheType, cacheKeyPrefix, cacheForMinutes, getDictionary);

            return dict.TryGetValue(lookUpCode, out var foundCode) ? (true, foundCode) : (false, lookUpCode);
        }

        protected async Task<Dictionary<string, string>> GetMappedCodesAsync(
            Guid filterId,
            RedisCacheType cacheType,
            string cacheKeyPrefix,
            int cacheForMinutes,
            Func<Guid, Task<Dictionary<string, string>>> getDictionary)
        {
            Dictionary<string, string> dict;
            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, filterId, cacheKeyPrefix);

            if (value.HasValue)
            {
                dict = _json.DeserializeFromString<Dictionary<string, string>>(value);
            }
            else
            {
                dict = await getDictionary(filterId);

                await _cacheManager.SafeCacheHashSetAsync(cacheType,
                    filterId,
                    new HashEntry[]
                    {
                        new(cacheKeyPrefix, _json.SerializeToString(dict))
                    },
                    TimeSpan.FromMinutes(cacheForMinutes));
            }

            return dict;
        }
    }
}