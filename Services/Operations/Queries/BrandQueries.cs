using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Services.Data;

namespace TGS.Services.Operations.Queries
{
    public interface IBrandQueries
    {
        Task<GetCachedBrandResult> GetCachedBrandAsync(Guid brandId);

        Task<GetCachedBrandGameProviderConfigurationResult> GetCachedBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);
    }

    public sealed class BrandQueries : IBrandQueries
    {
        private readonly TgsRepository _repository;

        public BrandQueries(TgsRepository repository)
        {
            _repository = repository;
        }

        Task<GetCachedBrandResult> IBrandQueries.GetCachedBrandAsync(Guid brandId)
        {
            const string spName = "usp_Brand_GetBrandForCache";
            return _repository.SqlQuerySingleAsync<GetCachedBrandResult>(spName, parameters: new SqlParameter
            {
                ParameterName = "@brandId",
                Value = brandId,
                DbType = DbType.Guid
            });
        }

        Task<GetCachedBrandGameProviderConfigurationResult> IBrandQueries.GetCachedBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId)
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
    }
}