using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Promotion.Core
{
    public interface IPromotionParticipantService
    {
        Task<PromotionParticipant> GetPromotionParticipantAsync(Expression<Func<PromotionParticipant, bool>> predicate,
            Func<IQueryable<PromotionParticipant>,
                IQueryable<PromotionParticipant>> preQuery = null);

        IQueryable<PromotionParticipant> GetPromotionParticipantQueries(
            Func<IQueryable<PromotionParticipant>,
                IQueryable<PromotionParticipant>> preQuery = null,
            bool tracking = false);

        Task CreatePromotionParticipantAsync(Guid promotionId, string playerCode);

        Task<bool> ExistsAsync(Expression<Func<PromotionParticipant, bool>> predicate);
    }
}