using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Generics;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IGameProviderQueries
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);

        Task<Dictionary<string, string>> GetGameProviderCurrenciesByExternalCodeAsync(Guid gameProviderId);

        Task<Dictionary<string, string>> GetGameProviderCurrenciesByInternalCodeAsync(Guid gameProviderId);

        Task<GetCachedGameProviderResult> GetCachedGameProviderAsync(Guid gameProviderId);

        Task<GetCachedGameProviderConfigurationResult> GetCachedGameProviderConfigurationAsync(
            Guid gameProviderConfigurationId);

        Task<Dictionary<string, string>> GetGameProviderLanguagesByExternalCodeAsync(Guid gameProviderId);

        Task<Dictionary<string, string>> GetGameProviderLanguagesByInternalCodeAsync(Guid gameProviderId);

        //Task<string> GetGameProviderBetLimitCodeAsync(Guid gameProviderId, string currencyCode, int betLimitGroupId);

        Task<CachedGame[]> GetCachedGamesAsync(Guid gameProviderId);

        Task<IdMap<int>[]> GetSubGameIdMapsAsync(Guid gameProviderId, PlatformType platformType);

        Task<IdAndCodeAndConfigurationId> GetGameProviderIdsByCredentialsAsync(string clientId, string secretKey);
    }

    public sealed class GameProviderQueries : IGameProviderQueries
    {
        private readonly ITgsRepository _repository;

        public GameProviderQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<Guid> IGameProviderQueries.GetGameProviderIdByCodeAsync(string gameProviderCode) =>
            _repository.GameProviders.Where(gp => gp.Code == gameProviderCode)
                .Select(gp => gp.Id).FirstOrDefaultAsync();

        Task<GetCachedGameProviderResult> IGameProviderQueries.GetCachedGameProviderAsync(Guid gameProviderId)
        {
            const string spName = "dbo.game_provider_get_game_provider_for_cache";
            return _repository.SqlQuerySingleAsync<GetCachedGameProviderResult>(spName,
                parameters: new Dictionary<string, object> {{":game_provider_id", gameProviderId}});
        }

        Task<GetCachedGameProviderConfigurationResult> IGameProviderQueries.GetCachedGameProviderConfigurationAsync(
            Guid gameProviderConfigurationId)
        {
            const string spName = "dbo.game_provider_get_game_provider_configuration_for_cache";
            return _repository.SqlQuerySingleAsync<GetCachedGameProviderConfigurationResult>(spName,
                parameters: new Dictionary<string, object>
                {
                    {":game_provider_configuration_id", gameProviderConfigurationId}
                });
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderCurrenciesByExternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "dbo.game_provider_get_currencies_by_external_code";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName,
                parameters: new Dictionary<string, object> {{":game_provider_id", gameProviderId}});

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderCurrenciesByInternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "dbo.game_provider_get_currencies_by_internal_code";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName,
                parameters: new Dictionary<string, object> {{":game_provider_id", gameProviderId}});

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderLanguagesByExternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "dbo.game_provider_get_languages_by_external_code";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName,
                parameters: new Dictionary<string, object> {{":game_provider_id", gameProviderId}});

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderLanguagesByInternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "dbo.game_provider_get_languages_by_internal_code";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName,
                parameters: new Dictionary<string, object> {{":game_provider_id", gameProviderId}});

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        //Task<string> IGameProviderQueries.GetGameProviderBetLimitCodeAsync(Guid gameProviderId, string currencyCode,
        //    int betLimitGroupId)
        //{
        //    const string spName = "usp_GameProvider_GetGameProviderBetLimitCode";
        //    return _repository.SqlQuerySingleAsync<string>(spName, parameters:
        //        new NpgsqlParameter[]
        //        {
        //            new()
        //            {
        //                ParameterName = "@gameProviderId",
        //                Value = gameProviderId,
        //                DbType = DbType.Guid
        //            },
        //            new()
        //            {
        //                ParameterName = "@currencyCode",
        //                Value = currencyCode,
        //                DbType = DbType.String
        //            },
        //            new()
        //            {
        //                ParameterName = "@betLimitGroupId",
        //                Value = betLimitGroupId,
        //                DbType = DbType.Int32
        //            }
        //        });
        //}

        async Task<CachedGame[]> IGameProviderQueries.GetCachedGamesAsync(Guid gameProviderId)
        {
            const string spName = "dbo.game_provider_get_games_for_cache";
            var r = await _repository.SqlQueryMultipleAsync<GetCachedGameResult>(spName,
                parameters: new Dictionary<string, object> {{":game_provider_id", gameProviderId}});

            return r.Select(g => new CachedGame
            {
                Id = g.Id,
                Code = g.Code,
                Name = g.Name,
                Description = g.Description,
                Status = g.Status,
                ExternalId = g.ExternalId,
                SubGame = g.SubGame,
                AvailableToBrand = g.AvailableToBrand,
                RuleUrl = g.RuleUrl,
                EndpointPath = g.EndpointPath,
                Type = g.Type,
                PlatformType = g.PlatformType,
                HasTurnoverOnTieBet = g.HasTurnoverOnTieBet,
                AvailableCurrencies = g.AvailableCurrencies.IsNotNullOrEmpty()
                    ? g.AvailableCurrencies.SplitByComma()
                    : Array.Empty<string>()
            }).ToArray();
        }

        async Task<IdMap<int>[]> IGameProviderQueries.GetSubGameIdMapsAsync(Guid gameProviderId,
            PlatformType platformType)
        {
            const string spName = "dbo.game_provider_get_sub_game_id_maps";
            var r = await _repository.SqlQueryMultipleAsync<IdMap<int>>(spName,
                parameters: new Dictionary<string, object>
                {
                    {":game_provider_id", gameProviderId},
                    {":platform_type", (short)platformType}
                });

            return r.ToArray();
        }

        Task<IdAndCodeAndConfigurationId> IGameProviderQueries.GetGameProviderIdsByCredentialsAsync(string clientId,
            string secretKey)
        {
            var statuses = new[] {Status.Active, Status.Maintenance};

            return (from gp in _repository.GameProviders
                    where statuses.Contains(gp.Status)
                    join gpc in _repository.GameProviderConfigurations
                        on gp.Id equals gpc.GameProviderId
                    where gpc.ClientId == clientId &&
                        gpc.SecretKey == secretKey &&
                        statuses.Contains(gpc.Status)
                    select new IdAndCodeAndConfigurationId
                    {
                        Id = gp.Id,
                        Code = gp.Code,
                        ConfigurationId = gpc.Id
                    })
                .FirstOrDefaultAsync();
        }
    }
}