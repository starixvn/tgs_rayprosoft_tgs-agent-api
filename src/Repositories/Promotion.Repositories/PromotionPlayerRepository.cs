using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion;

namespace TGS.Promotion.Repositories
{
    public class PromotionPlayerRepository : BaseRepositoryOperations<ITgsRepository>,
        IPromotionPlayerRepository
    {
        private readonly ITgsRepository _repository;

        public PromotionPlayerRepository(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
        }

        IQueryable<PromotionPlayer> IPromotionPlayerRepository.GetPromotionPlayerQueries(
            Func<IQueryable<PromotionPlayer>,
                IQueryable<PromotionPlayer>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        Task<PromotionPlayer> IPromotionPlayerRepository.GetPromotionPlayerAsync(
            Expression<Func<PromotionPlayer, bool>> predicate,
            Func<IQueryable<PromotionPlayer>,
                IQueryable<PromotionPlayer>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);
    }
}