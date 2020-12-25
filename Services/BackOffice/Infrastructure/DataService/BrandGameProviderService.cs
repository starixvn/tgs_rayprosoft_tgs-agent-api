using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IBrandGameProviderService
    {
        Task<BrandGameProvider> GetBrandGameProviderAsync(
            Expression<Func<BrandGameProvider, bool>> predicate,
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery =
                null);

        IQueryable<BrandGameProvider> GetBrandGameProvidersAsync(
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery = null,
            bool tracking = false);

        Task UpdateBrandGameProviderAsync(BrandGameProvider schedule);
    }

    public class BrandGameProviderService : BaseService, IBrandGameProviderService
    {
        public BrandGameProviderService(ITgsRepository tgsRepository) : base(tgsRepository)
        {
        }

        Task<BrandGameProvider> IBrandGameProviderService.GetBrandGameProviderAsync(
            Expression<Func<BrandGameProvider, bool>> predicate,
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<BrandGameProvider> IBrandGameProviderService.GetBrandGameProvidersAsync(
            Func<IQueryable<BrandGameProvider>, IQueryable<BrandGameProvider>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task IBrandGameProviderService.UpdateBrandGameProviderAsync(
            BrandGameProvider schedule) =>
            UpdateEntityAsync(schedule);
    }
}