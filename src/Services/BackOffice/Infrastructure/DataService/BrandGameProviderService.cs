using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IBrandGameProviderService
    {
        Task<BrandGameProvider> GetBrandGameProviderAsync(
            Expression<Func<BrandGameProvider, bool>> predicate,
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery =
                null);

        IQueryable<BrandGameProvider> GetBrandGameProviderQueries(
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery = null,
            bool tracking = false);

        Task UpdateBrandGameProviderAsync(BrandGameProvider brandGameProvider);
        Task<bool> UpdateBrandGameProvidersAsync(string brandGameProviderIdsString, Status status);
    }

    public class BrandGameProviderService : BaseRepositoryOperations<ITgsRepository>,
        IBrandGameProviderService
    {
        private readonly ITgsRepository _repository;

        public BrandGameProviderService(ITgsRepository tgsRepository) : base(tgsRepository)
            => _repository = tgsRepository;

        Task<BrandGameProvider> IBrandGameProviderService.GetBrandGameProviderAsync(
            Expression<Func<BrandGameProvider, bool>> predicate,
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<BrandGameProvider> IBrandGameProviderService.GetBrandGameProviderQueries(
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task IBrandGameProviderService.UpdateBrandGameProviderAsync(
            BrandGameProvider brandGameProvider) =>
            UpdateEntityAsync(brandGameProvider);

        Task<bool> IBrandGameProviderService.UpdateBrandGameProvidersAsync(string brandGameProviderIdsString,
            Status status)
        {
            const string spName = "dbo.update_brand_game_provider_status";
            return _repository
                .SqlQuerySingleAsync<bool>(spName,
                    parameters:
                    new Dictionary<string, object>
                    {
                        { "_ids", brandGameProviderIdsString },
                        { "_status", status }
                    });
        }
    }
}