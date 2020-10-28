using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Generics;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface ILicenseeQueries
    {
        Task<GetCachedLicenseeResult> GetCachedLicenseeAsync(Guid licenseeId);

        Task<GetCachedLicenseeGameProviderConfigurationResult> GetCachedLicenseeGameProviderConfigurationAsync(
            Guid licenseeId, Guid gameProviderId);

        Task<CachedLicenseeGame[]> GetLicenseeGamesAsync(Guid licenseeId);

        Task<Guid> GetLicenseeIdByClientIdAsync(string clientId);

        Task<Dictionary<string, string>> GetLicenseeLanguagesByExternalCodeAsync(Guid licenseeId);

        Task<Dictionary<string, string>> GetLicenseeLanguagesByInternalCodeAsync(Guid licenseeId);

        Task<Dictionary<string, string>> GetLicenseeCurrenciesByExternalCodeAsync(Guid licenseeId);

        Task<Dictionary<string, string>> GetLicenseeCurrenciesByInternalCodeAsync(Guid licenseeId);

        Task<CachedLicenseeGameProvider[]> GetLicenseeGameProvidersAsync(Guid licenseeId, string currencyCode, PlatformType platformType,
            GameType[] gameTypes);
    }

    public sealed class LicenseeQueries : ILicenseeQueries
    {
        private readonly ITgsRepository _repository;

        public LicenseeQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<GetCachedLicenseeResult> ILicenseeQueries.GetCachedLicenseeAsync(Guid licenseeId)
        {
            const string spName = "usp_Licensee_GetLicenseeForCache";
            return _repository.SqlQuerySingleAsync<GetCachedLicenseeResult>(spName, parameters: new SqlParameter
            {
                ParameterName = "@licenseeId",
                Value = licenseeId,
                DbType = DbType.Guid
            });
        }

        Task<GetCachedLicenseeGameProviderConfigurationResult> ILicenseeQueries.
            GetCachedLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId)
        {
            const string spName = "usp_Licensee_GetLicenseeGameProviderConfigurationForCache";
            return _repository.SqlQuerySingleAsync<GetCachedLicenseeGameProviderConfigurationResult>(spName, parameters:
                new SqlParameter[]
                {
                    new ()
                    {
                        ParameterName = "@licenseeId",
                        Value = licenseeId,
                        DbType = DbType.Guid
                    },
                    new ()
                    {
                        ParameterName = "@gameProviderId",
                        Value = gameProviderId,
                        DbType = DbType.Guid
                    }
                });
        }

        async Task<CachedLicenseeGame[]> ILicenseeQueries.GetLicenseeGamesAsync(Guid licenseeId)
        {
            const string spName = "usp_Licensee_GetLicenseeGamesForCache";
            var r = await _repository.SqlQueryMultipleAsync<CachedLicenseeGame>(spName, parameters: new SqlParameter
            {
                ParameterName = "@licenseeId",
                Value = licenseeId,
                DbType = DbType.Guid
            });

            return r.ToArray();
        }

        Task<Guid> ILicenseeQueries.GetLicenseeIdByClientIdAsync(string clientId)
        {
            return _repository.Licensees.Where(l => l.ClientId == clientId)
                .Select(l => l.Id).FirstOrDefaultAsync();
        }

        async Task<Dictionary<string, string>> ILicenseeQueries.GetLicenseeLanguagesByExternalCodeAsync(Guid licenseeId)
        {
            const string spName = "usp_Licensee_GetLanguagesByExternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@licenseeId",
                Value = licenseeId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> ILicenseeQueries.GetLicenseeLanguagesByInternalCodeAsync(Guid licenseeId)
        {
            const string spName = "usp_Licensee_GetLanguagesByInternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@licenseeId",
                Value = licenseeId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> ILicenseeQueries.GetLicenseeCurrenciesByExternalCodeAsync(
            Guid licenseeId)
        {
            const string spName = "usp_Licensee_GetCurrenciesByExternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@licenseeId",
                Value = licenseeId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<Dictionary<string, string>> ILicenseeQueries.GetLicenseeCurrenciesByInternalCodeAsync(
            Guid licenseeId)
        {
            const string spName = "usp_Licensee_GetCurrenciesByInternalCode";
            var r = await _repository.SqlQueryMultipleAsync<Map<string, string>>(spName, parameters: new SqlParameter
            {
                ParameterName = "@licenseeId",
                Value = licenseeId,
                DbType = DbType.Guid
            });

            return r.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
        }

        async Task<CachedLicenseeGameProvider[]> ILicenseeQueries.GetLicenseeGameProvidersAsync(Guid licenseeId,
            string currencyCode, PlatformType platformType, GameType[] gameTypes)
        {
            const string spName = "usp_Licensee_GetLicenseeGameProviders";
            var r = await _repository.SqlQueryMultipleAsync<CachedLicenseeGameProvider>(spName, parameters: new SqlParameter[]
            {
                new ()
                {
                    ParameterName = "@licenseeId",
                    Value = licenseeId,
                    DbType = DbType.Guid
                },
                new ()
                {
                    ParameterName = "@currencyCode",
                    Value = currencyCode,
                    DbType = DbType.String
                },
                new ()
                {
                    ParameterName = "@platformType",
                    Value = platformType,
                    DbType = DbType.Int16
                },
                new ()
                {
                    ParameterName = "@gameTypes",
                    Value = gameTypes.Select(s => new {Value = (int) s}).ToDataTable(),
                    SqlDbType = SqlDbType.Structured
                }
            });

            return r.ToArray();
        }
    }
}