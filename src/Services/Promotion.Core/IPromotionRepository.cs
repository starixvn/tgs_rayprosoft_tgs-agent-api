using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.Models.Settings;
using TGS.Services.Promotion.Core.Models.Promotion;

namespace TGS.Services.Promotion.Core
{
    public interface IPromotionRepository
    {
        Task<TGS.Core.DataServices.Tgs.Models.Promotion> GetPromotionAsync(
            Expression<Func<TGS.Core.DataServices.Tgs.Models.Promotion, bool>> predicate,
            Func<IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>,
                IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>> preQuery = null);

        IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion> GetPromotionQueries(
            Func<IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>,
                IQueryable<TGS.Core.DataServices.Tgs.Models.Promotion>> preQuery = null,
            bool tracking = false);

        Task CreatePromotionAsync(CreatePromotionRequest request, PromotionType promotionTypeModel);
        Task UpdatePromotionAsync(TGS.Core.DataServices.Tgs.Models.Promotion promotion, UpdatePromotionRequest request);
        Task ChangePromotionStatusAsync(TGS.Core.DataServices.Tgs.Models.Promotion promotion);
        Task<bool> ExistsAsync(Expression<Func<TGS.Core.DataServices.Tgs.Models.Promotion, bool>> predicate);
    }
}