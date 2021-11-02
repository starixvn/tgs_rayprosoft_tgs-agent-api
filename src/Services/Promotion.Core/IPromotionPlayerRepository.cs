using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Promotion
{
    public interface IPromotionPlayerRepository
    {
        Task<PromotionPlayer> GetPromotionPlayerAsync(
            Expression<Func<PromotionPlayer, bool>> predicate,
            Func<IQueryable<PromotionPlayer>, IQueryable<PromotionPlayer>>? preQuery = null);

        IQueryable<PromotionPlayer> GetPromotionPlayerQueries(
            Func<IQueryable<PromotionPlayer>, IQueryable<PromotionPlayer>>? preQuery = null,
            bool tracking = false);
    }
}