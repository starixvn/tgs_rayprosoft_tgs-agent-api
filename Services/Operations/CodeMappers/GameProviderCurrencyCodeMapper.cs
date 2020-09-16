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
    public interface IGameProviderCurrencyCodeMapper : ICodeMapper
    {

    }

    public sealed class GameProviderCurrencyCodeMapper : BaseCodeMapper, IGameProviderCurrencyCodeMapper
    {
        private readonly Func<IGameProviderQueries> _gameProviderQueriesFactory;

        public GameProviderCurrencyCodeMapper(
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
                RedisCacheType.Currencies,
                Consts.CachePrefix.GameProviderToTgsMap,
                "currency",
                30,
                id => _gameProviderQueriesFactory().GetGameProviderCurrenciesByExternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code \'{externalCode}\' for \'{Consts.CachePrefix.GameProviderToTgsMap}\' used for \'currency\' with filter id {gameProviderId} (intended to be cached as {RedisCacheType.Currencies})");
            }

            return code;
        }

        async Task<string> ICodeMapper.InternalToExternalAsync(string internalCode, Guid gameProviderId)
        {
            var (found, code) = await GetMappedCodeAsync(
                internalCode,
                gameProviderId,
                RedisCacheType.Currencies,
                Consts.CachePrefix.TgsToGameProviderMap,
                "currency",
                30,
                id => _gameProviderQueriesFactory().GetGameProviderCurrenciesByInternalCodeAsync(id));

            if (!found)
            {
                throw new InvalidOperationException(
                    $"Not found code \'{internalCode}\' for \'{Consts.CachePrefix.TgsToGameProviderMap}\' used for \'currency\' with filter id {gameProviderId} (intended to be cached as {RedisCacheType.Currencies})");
            }

            return code;
        }
    }
}
