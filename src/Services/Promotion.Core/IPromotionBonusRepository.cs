using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Promotion.Models.PromotionBonus;

namespace TGS.Services.Promotion
{
    public interface IPromotionBonusRepository
    {
        ITgsRepository Repository { get; }

        Task CreatePromotionBonusAsync(PromotionBonusModel[] model);

        IQueryable<PromotionPlayer> GetPromotionBonusPlayersQueries(
            Func<IQueryable<PromotionPlayer>, IQueryable<PromotionPlayer>> preQuery = null,
            bool tracking = false);

        IQueryable<PromotionBonus> GetPromotionBonusQueries(
                            Func<IQueryable<PromotionBonus>, IQueryable<PromotionBonus>> preQuery = null,
            bool tracking = false);

        Task UpdatePromotionBonusPlayersAsync(PromotionPlayer[] players);
    }
}
