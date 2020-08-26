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
    }
}
