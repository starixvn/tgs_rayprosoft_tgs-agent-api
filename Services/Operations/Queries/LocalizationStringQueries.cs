using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface ILocalizationStringQueries
    {
        Task<LocalizedStringResult[]> GetLocalizationByGameProviderIdAsync(Guid gameProviderId);
    }

    public sealed class LocalizationStringQueries : ILocalizationStringQueries
    {
        private readonly ITgsRepository _repository;

        public LocalizationStringQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        async Task<LocalizedStringResult[]> ILocalizationStringQueries.GetLocalizationByGameProviderIdAsync(
            Guid gameProviderId)
        {
            const string spName = "usp_Localization_GetLocalizationByGameProviderId";
            var r = await _repository.SqlQueryMultipleAsync<LocalizedStringResult>(spName, parameters: new SqlParameter
            {
                ParameterName = "@gameProviderId",
                Value = gameProviderId,
                DbType = DbType.Guid
            });

            return r.ToArray();
        }
    }
}