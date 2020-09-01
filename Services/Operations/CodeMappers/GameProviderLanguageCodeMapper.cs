using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Infrastructures.CodeMapper;
using TGS.Infrastructures.Json;
using TGS.Infrastructures.Redis;
using TGS.Services.Operations.Queries;

namespace TGS.Services.Operations.CodeMappers
{
    public interface IGameProviderLanguageCodeMapper : ICodeMapper
    {

    }

    public sealed class GameProviderLanguageCodeMapper : BaseCodeMapper, IGameProviderLanguageCodeMapper
    {
        private readonly Func<IGameProviderQueries> _gameProviderQueriesFactory;

        public GameProviderLanguageCodeMapper(
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            Func<IGameProviderQueries> gameProviderQueriesFactory) : base(cacheManager, json)
        {
            _gameProviderQueriesFactory = gameProviderQueriesFactory;
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
                id => _gameProviderQueriesFactory().GetGameProviderLanguagesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code \'{externalCode}\' for \'{Consts.CachePrefix.GameProviderToTgsMap}\' used for \'language\' with filter id {gameProviderId} (intended to be cached as {RedisCacheType.Languages})");
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
                id => _gameProviderQueriesFactory().GetGameProviderLanguagesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code \'{internalCode}\' for \'{Consts.CachePrefix.TgsToGameProviderMap}\' used for \'language\' with filter id {gameProviderId} (intended to be cached as {RedisCacheType.Languages})");
            }

            return code;
        }
    }
}
