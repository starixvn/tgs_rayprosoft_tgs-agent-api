using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using TGS.Core.Models.Enums;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Redis;

namespace TGS.Infrastructures.CodeMapper
{
    public interface ICodeMapper
    {
        Task<string> ExternalToInternalAsync(string externalCode, Guid id);
        Task<string> InternalToExternalAsync(string internalCode, Guid id);
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

        protected virtual async Task<(bool Found, string Code)> GetMappedCodeAsync(
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
                    $"Missing key for \'{cacheKeyPrefix}\' used for \'{usedFor}\' with filter id {filterId} (intended to be cached as {cacheType})");
            }

            Dictionary<string, string> dict;
            var value = await _cacheManager.GetHashSetAsync(cacheType, filterId, cacheKeyPrefix);
            
            if (value.HasValue)
            {
                dict = _json.DeserializeFromString<Dictionary<string, string>>(value);
            }
            else
            {
                dict = await getDictionary(filterId);

                await _cacheManager.CacheHashSetAsync(cacheType, filterId, new HashEntry[]
                {
                    new (cacheKeyPrefix, _json.SerializeToString(dict))
                }, TimeSpan.FromMinutes(cacheForMinutes));
            }

            return dict.TryGetValue(lookUpCode, out var foundCode) ? (true, foundCode) : (false, lookUpCode);
        }
    }
}
