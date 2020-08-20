using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
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

        protected BaseCodeMapper(IRedisCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
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

            var dict =
                await _cacheManager.SafeGetAndCacheAsync(
                    cacheType, $"{cacheKeyPrefix}:{filterId}", TimeSpan.FromMinutes(cacheForMinutes),
                    () => getDictionary(filterId));

            return dict.TryGetValue(lookUpCode, out var foundCode) ? (true, foundCode) : (false, lookUpCode);
        }
    }
}
