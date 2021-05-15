using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsBrandService
    {
        ITgsRepository Repository { get; }

        IQueryable<Brand> GetBrandQueries(
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery = null,
            bool tracking = false);

        Task UpdateBrandsAsync(IEnumerable<Brand> brands);
    }

    public class TgsBrandService : BaseRepositoryOperations<ITgsRepository>, ITgsBrandService
    {
        private readonly ITgsRepository _repository;

        public TgsBrandService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository ITgsBrandService.Repository => _repository;

        IQueryable<Brand> ITgsBrandService.GetBrandQueries(
            Func<IQueryable<Brand>, IQueryable<Brand>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task ITgsBrandService.UpdateBrandsAsync(IEnumerable<Brand> brands) => UpdateEntitiesAsync(brands);
    }
}