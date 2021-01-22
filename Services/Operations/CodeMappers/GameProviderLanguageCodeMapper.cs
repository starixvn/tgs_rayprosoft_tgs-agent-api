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
    public interface IGameProviderLanguageCodeMapper : ICodeMapper
    {
    }

    public sealed class GameProviderLanguageCodeMapper : BaseCodeMapper, IGameProviderLanguageCodeMapper
    {
        private readonly IGameProviderQueries _gameProviderQueries;

        public GameProviderLanguageCodeMapper(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IGameProviderQueries gameProviderQueries) : base(cacheManager, json)
        {
            _gameProviderQueries = gameProviderQueries;
        }

        async Task<string> ICodeMapper.ExternalToInternalAsync(string externalCode, Guid gameProviderId)
        {
            var (found, code) = await GetMappedCodeAsync(
                externalCode,
                gameProviderId,
                RedisCacheType.Languages,
                Consts.CachePrefix.GameProviderToTgsMap,
                "language",
                30,
                id => _gameProviderQueries.GetGameProviderLanguagesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{externalCode}' for '{Consts.CachePrefix.GameProviderToTgsMap}' used for 'language' with filter id '{gameProviderId}' (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }

        async Task<string> ICodeMapper.InternalToExternalAsync(string internalCode, Guid gameProviderId)
        {
            var (found, code) = await GetMappedCodeAsync(
                internalCode,
                gameProviderId,
                RedisCacheType.Languages,
                Consts.CachePrefix.TgsToGameProviderMap,
                "language",
                30,
                id => _gameProviderQueries.GetGameProviderLanguagesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code '{internalCode}' for '{Consts.CachePrefix.TgsToGameProviderMap}' used for 'language' with filter id {gameProviderId} (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }
    }
}