using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Settings;
using TGS.Services.PromotionCore.Models.Promotions;

namespace TGS.Services.PromotionCore
{
    public interface IPromotionService
    {
        Task<Promotion> GetPromotionAsync(Expression<Func<Promotion, bool>> predicate,
            Func<IQueryable<Promotion>,
                IQueryable<Promotion>> preQuery = null);

        IQueryable<Promotion> GetPromotionQueries(
            Func<IQueryable<Promotion>,
                IQueryable<Promotion>> preQuery = null,
            bool tracking = false);

        Task CreatePromotionAsync(CreatePromotionRequest request, PromotionType promotionTypeModel);
        Task UpdatePromotionAsync(Promotion promotion, UpdatePromotionRequest request);
        Task<bool> ExistsAsync(Expression<Func<Promotion, bool>> predicate);
    }
}