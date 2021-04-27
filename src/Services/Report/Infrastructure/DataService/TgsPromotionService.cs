using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsPromotionService
    {
        ITgsRepository TgsRepository { get; }

        Task UpdatePromotionsAsync(Promotion[] promotions);

        Task<Promotion> GetPromotionAsync(Expression<Func<Promotion, bool>> predicate,
            Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery = null);

        IQueryable<Promotion> GetPromotionQueries(Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery = null,
            bool tracking = false);
    }

    public class TgsPromotionService : BaseRepositoryOperations<ITgsRepository>, ITgsPromotionService
    {
        public readonly ITgsRepository _repository;

        public TgsPromotionService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository ITgsPromotionService.TgsRepository => _repository;

        Task<Promotion> ITgsPromotionService.GetPromotionAsync(Expression<Func<Promotion, bool>> predicate,
            Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Promotion> ITgsPromotionService.GetPromotionQueries(
            Func<IQueryable<Promotion>, IQueryable<Promotion>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task ITgsPromotionService.UpdatePromotionsAsync(Promotion[] promotions) => UpdateEntitiesAsync(promotions);
    }
}