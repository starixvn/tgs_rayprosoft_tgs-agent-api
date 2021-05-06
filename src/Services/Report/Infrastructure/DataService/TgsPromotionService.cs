using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsPromotionService
    {
        ITgsRepository TgsRepository { get; }

        Task UpdatePromotionsAsync(Core.DataServices.Tgs.Models.Promotion[] promotions);

        Task<Core.DataServices.Tgs.Models.Promotion> GetPromotionAsync(
            Expression<Func<Core.DataServices.Tgs.Models.Promotion, bool>> predicate,
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>, IQueryable<Core.DataServices.Tgs.Models.Promotion>>
                preQuery = null);

        IQueryable<Core.DataServices.Tgs.Models.Promotion> GetPromotionQueries(
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>, IQueryable<Core.DataServices.Tgs.Models.Promotion>>
                preQuery = null,
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

        Task<Core.DataServices.Tgs.Models.Promotion> ITgsPromotionService.GetPromotionAsync(
            Expression<Func<Core.DataServices.Tgs.Models.Promotion, bool>> predicate,
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>, IQueryable<Core.DataServices.Tgs.Models.Promotion>>
                preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Core.DataServices.Tgs.Models.Promotion> ITgsPromotionService.GetPromotionQueries(
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>, IQueryable<Core.DataServices.Tgs.Models.Promotion>>
                preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task ITgsPromotionService.UpdatePromotionsAsync(Core.DataServices.Tgs.Models.Promotion[] promotions) =>
            UpdateEntitiesAsync(promotions);
    }
}