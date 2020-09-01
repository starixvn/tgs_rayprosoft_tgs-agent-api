using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Generics;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IGameProviderQueries
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);

        Task<Dictionary<string, string>> GetGameProviderCurrenciesByExternalCodeAsync(Guid gameProviderId);

        Task<Dictionary<string, string>> GetGameProviderCurrenciesByInternalCodeAsync(Guid gameProviderId);

        Task<GetCachedGameProviderResult> GetCachedGameProviderAsync(Guid gameProviderId);

        Task<GetCachedGameProviderConfigurationResult> GetCachedGameProviderConfigurationAsync(Guid gameProviderConfigurationId);
        
        Task<Dictionary<string, string>> GetGameProviderLanguagesByExternalCodeAsync(Guid gameProviderId);
        
        Task<Dictionary<string, string>> GetGameProviderLanguagesByInternalCodeAsync(Guid gameProviderId);
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

        Task<GetCachedGameProviderConfigurationResult> IGameProviderQueries.GetCachedGameProviderConfigurationAsync(Guid gameProviderConfigurationId)
        {
            const string spName = "usp_GameProvider_GetGameProviderConfigurationForCache";
            return _repository.SqlQuerySingleAsync<GetCachedGameProviderConfigurationResult>(spName, parameters: new SqlParameter
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

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderLanguagesByExternalCodeAsync(Guid gameProviderId)
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

        async Task<Dictionary<string, string>> IGameProviderQueries.GetGameProviderLanguagesByInternalCodeAsync(Guid gameProviderId)
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
    }
}