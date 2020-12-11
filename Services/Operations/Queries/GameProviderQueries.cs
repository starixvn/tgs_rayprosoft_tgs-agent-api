using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
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

        Task<string> GetGameProviderBetLimitCodeAsync(Guid gameProviderId, string currencyCode, int betLimitGroupId);

        Task<CachedGame[]> GetCachedGamesAsync(Guid gameProviderId);

        Task<IdMap<int>[]> GetSubGameIdMapsAsync(Guid gameProviderId, PlatformType platformType);
    }

    public sealed class GameProviderQueries : IGameProviderQueries
    {
        private readonly ITgsRepository _repository;

        public GameProviderQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<Guid> IGameProviderQueries.GetGameProviderIdByCodeAsync(string gameProviderCode)
        {
            const string spName = "usp_GameProvider_GetGameProviderIdByCode";
            return _repository.SqlQuerySingleAsync<Guid>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderCode",
                Value = gameProviderCode,
                DbType = DbType.String
            });
        }

        Task<GetCachedGameProviderResult> IGameProviderQueries.GetCachedGameProviderAsync(Guid gameProviderId)
        {
            const string spName = "usp_GameProvider_GetGameProviderForCache";
            return _repository.SqlQuerySingleAsync<GetCachedGameProviderResult>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });
        }

        Task<GetCachedGameProviderConfigurationResult> IGameProviderQueries.GetCachedGameProviderConfigurationAsync(
            Guid gameProviderConfigurationId)
        {
            const string spName = "usp_GameProvider_GetGameProviderConfigurationForCache";
            return _repository.SqlQuerySingleAsync<GetCachedGameProviderConfigurationResult>(spName,
                parameters: new SqlParameter
                {
                    ParameterName = "@gameProviderConfigurationId",
                    Value = gameProviderConfigurationId,
                    DbType = DbType.Guid
                });
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderCurrenciesByExternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "usp_GameProvider_GetCurrenciesByExternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderCurrenciesByInternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "usp_GameProvider_GetCurrenciesByInternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderLanguagesByExternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "usp_GameProvider_GetLanguagesByExternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderLanguagesByInternalCodeAsync(
            Guid gameProviderId)
        {
            const string spName = "usp_GameProvider_GetLanguagesByInternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        Task<string> IGameProviderQueries.GetGameProviderBetLimitCodeAsync(Guid gameProviderId, string currencyCode,
            int betLimitGroupId)
        {
            const string spName = "usp_GameProvider_GetGameProviderBetLimitCode";
            return _repository.SqlQuerySingleAsync<string>(spName, parameters:
                new SqlParameter[]
                {
                    new()
                    {
                        ParameterName = "@gameProviderId",
                        Value = gameProviderId,
                        DbType = DbType.Guid
                    },
                    new()
                    {
                        ParameterName = "@currencyCode",
                        Value = currencyCode,
                        DbType = DbType.String
                    },
                    new()
                    {
                        ParameterName = "@betLimitGroupId",
                        Value = betLimitGroupId,
                        DbType = DbType.Int32
                    }
                });
        }

        async Task<CachedGame[]> IGameProviderQueries.GetCachedGamesAsync(Guid gameProviderId)
        {
            const string spName = "usp_GameProvider_GetGamesForCache";
            var r = await _repository.SqlQueryMultipleAsync<GetCachedGameResult>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });

            return r.Select(g => new CachedGame
            {
                Id = g.Id,
                Code = g.Code,
                Name = g.Name,
                Description = g.Description,
                Status = g.Status,
                ExternalId = g.ExternalId,
                SubGame = g.SubGame,
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

        async Task<IdMap<int>[]> IGameProviderQueries.GetSubGameIdMapsAsync(Guid gameProviderId, PlatformType platformType)
        {
            const string spName = "usp_GameProvider_GetSubGameIdMaps";
            var r = await _repository.SqlQueryMultipleAsync<IdMap<int>>(spName, parameters: new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@gameProviderId",
                    Value = gameProviderId,
                    DbType = DbType.Guid
                },
                new()
                {
                    ParameterName = "@platformType",
                    Value = platformType,
                    DbType = DbType.Int16
                }
            });

            return r.ToArray();
        }
    }
}