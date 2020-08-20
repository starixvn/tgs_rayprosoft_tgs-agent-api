using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.Models.Generics;
using TGS.Services.Operations.Extensions;

namespace TGS.Services.Operations.Queries
{
    public interface IGameProviderQueries
    {
        Task<Guid> GetGameProviderIdByCodeAsync(string gameProviderCode);
        Task<Dictionary<string, string>> GetGameProviderCurrenciesByExternalCodeAsync(Guid gameProviderId);
        Task<Dictionary<string, string>> GetGameProviderCurrenciesByInternalCodeAsync(Guid gameProviderId);
    }

    public sealed class GameProviderQueries : IGameProviderQueries
    {
        private readonly ITgsRepository _repository;

        public GameProviderQueries(ITgsRepository repository)
        {
            _repository = repository;
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

            return r.ToDictionary(m => m.Key, m => m.Value);
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

            return r.ToDictionary(m => m.Key, m => m.Value);
        }
    }
}