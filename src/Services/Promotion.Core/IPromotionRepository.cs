using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.Models.Settings;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Services.Promotion
{
    public interface IPromotionRepository
    {
        Task ChangePromotionStatusAsync(Core.DataServices.Tgs.Models.Promotion promotion);

        Task CreatePromotionAsync(CreatePromotionRequest request, PromotionType promotionTypeModel);

        Task<bool> ExistsAsync(Expression<Func<Core.DataServices.Tgs.Models.Promotion, bool>> predicate);

        Task<Core.DataServices.Tgs.Models.Promotion> GetPromotionAsync(
            Expression<Func<Core.DataServices.Tgs.Models.Promotion, bool>> predicate,
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>, IQueryable<Core.DataServices.Tgs.Models.Promotion>>
                preQuery = null);

        Task<Core.DataServices.Tgs.Models.Promotion> GetPromotionByIdAsync(Guid promotionId);

        IQueryable<Core.DataServices.Tgs.Models.Promotion> GetPromotionQueries(
            Func<IQueryable<Core.DataServices.Tgs.Models.Promotion>, IQueryable<Core.DataServices.Tgs.Models.Promotion>>
                preQuery = null,
            bool tracking = false);

        Task UpdatePromotionAsync(Core.DataServices.Tgs.Models.Promotion promotion, UpdatePromotionRequest request);
    }
}