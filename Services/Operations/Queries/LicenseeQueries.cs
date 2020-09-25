using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface ILicenseeQueries
    {
        Task<GetCachedLicenseeResult> GetCachedLicenseeAsync(Guid licenseeId);

        Task<GetCachedLicenseeGameProviderConfigurationResult> GetCachedLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId);

        Task<CachedLicenseeGame[]> GetLicenseeGamesAsync(Guid licenseeId);

        Task<Guid> GetLicenseeIdByClientIdAsync(string clientId);
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

        Task<GetCachedLicenseeGameProviderConfigurationResult> ILicenseeQueries.GetCachedLicenseeGameProviderConfigurationAsync(Guid licenseeId, Guid gameProviderId)
        {
            const string spName = "usp_Licensee_GetLicenseeGameProviderConfigurationForCache";
            return _repository.SqlQuerySingleAsync<GetCachedLicenseeGameProviderConfigurationResult>(spName, parameters:
            new SqlParameter[]
            {
                new()
                {
                    ParameterName = "@licenseeId",
                    Value = licenseeId,
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
    }
}