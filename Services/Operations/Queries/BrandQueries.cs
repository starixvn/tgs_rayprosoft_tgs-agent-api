using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Generics;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IBrandQueries
    {
        Task<CachedBrand> GetCachedBrandAsync(Guid brandId);

        Task<GetCachedBrandGameProviderConfigurationResult> GetCachedBrandGameProviderConfigurationAsync(
            Guid brandId, Guid gameProviderId);

        Task<CachedBrandGame[]> GetBrandGamesAsync(Guid brandId);

        Task<Guid> GetBrandIdByClientIdAsync(string clientId);

        Task<Guid> GetBrandIdByApiKeyAsync(string apiKey);

        Task<Dictionary<string, string>> GetBrandLanguagesByExternalCodeAsync(Guid brandId);

        Task<Dictionary<string, string>> GetBrandLanguagesByInternalCodeAsync(Guid brandId);

        Task<Dictionary<string, string>> GetBrandCurrenciesByExternalCodeAsync(Guid brandId);

        Task<Dictionary<string, string>> GetBrandCurrenciesByInternalCodeAsync(Guid brandId);

        Task<CachedBrandGameProvider[]> GetBrandGameProvidersAsync(Guid brandId);
    }

    public sealed class BrandQueries : IBrandQueries
    {
        private readonly ITgsRepository _repository;

        public BrandQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<CachedBrand> IBrandQueries.GetCachedBrandAsync(Guid brandId)
        {
            const string spName = "usp_Brand_GetBrandForCache";
            return _repository.SqlQuerySingleAsync<CachedBrand>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });
        }

        Task<GetCachedBrandGameProviderConfigurationResult> IBrandQueries.
            GetCachedBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId)
        {
            const string spName = "usp_Brand_GetBrandGameProviderConfigurationForCache";
            return _repository.SqlQuerySingleAsync<GetCachedBrandGameProviderConfigurationResult>(spName, parameters:
                new SqlParameter[]
                {
                    new()
                    {
                        ParameterName = "@brandId",
                        Value = brandId,
                        DbType = DbType.Guid
                    },
                    new()
                    {
                        ParameterName = "@gameProviderId",
                        Value = gameProviderId,
                        DbType = DbType.Guid
                    }
                });
        }

        async Task<CachedBrandGame[]> IBrandQueries.GetBrandGamesAsync(Guid brandId)
        {
            const string spName = "usp_Brand_GetBrandGamesForCache";
            var r = await _repository.SqlQueryMultipleAsync<CachedBrandGame>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });

            return r.ToArray();
        }

        Task<Guid> IBrandQueries.GetBrandIdByClientIdAsync(string clientId) =>
            _repository.Brands.Where(b => b.ClientId == clientId)
                .Select(b => b.Id).FirstOrDefaultAsync();

        Task<Guid> IBrandQueries.GetBrandIdByApiKeyAsync(string apiKey) =>
            _repository.Brands.Where(b => b.ApiKey == apiKey)
                .Select(b => b.Id).FirstOrDefaultAsync();

        async Task<Dictionary<string, string>> IBrandQueries.GetBrandLanguagesByExternalCodeAsync(Guid brandId)
        {
            const string spName = "usp_Brand_GetLanguagesByExternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IBrandQueries.GetBrandLanguagesByInternalCodeAsync(Guid brandId)
        {
            const string spName = "usp_Brand_GetLanguagesByInternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IBrandQueries.GetBrandCurrenciesByExternalCodeAsync(
            Guid brandId)
        {
            const string spName = "usp_Brand_GetCurrenciesByExternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> IBrandQueries.GetBrandCurrenciesByInternalCodeAsync(
            Guid brandId)
        {
            const string spName = "usp_Brand_GetCurrenciesByInternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<CachedBrandGameProvider[]> IBrandQueries.GetBrandGameProvidersAsync(Guid brandId)
        {
            const string spName = "usp_Brand_GetBrandGameProvidersForCache";
            var r = await _repository.SqlQueryMultipleAsync<CachedBrandGameProvider>(spName,
                parameters: new SqlParameter[]
                {
                    new()
                    {
                        ParameterName = "@brandId",
                        Value = brandId,
                        DbType = DbType.Guid
                    }
                });

            return r.ToArray();
        }
    }
}