using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.PromotionCore.Models.PromotionBonus;
using TGS.Services.PromotionCore.Models.PromotionDispatch;

namespace TGS.Services.PromotionCore
{
    public interface IPromotionBonusService
    {
        ITgsRepository Repository { get; }

        IQueryable<PromotionBonus> GetPromotionBonusQueries(
            Func<IQueryable<PromotionBonus>, IQueryable<PromotionBonus>> preQuery = null,
            bool tracking = false);

        IQueryable<PromotionPlayer> GetPromotionBonusPlayersQueries(
            Func<IQueryable<PromotionPlayer>, IQueryable<PromotionPlayer>> preQuery = null,
            bool tracking = false);

        Task CreatePromotionBonusAsync(PromotionBonusModel[] model);

        Task DispatchPromotionBonus(DispatchPromotionBonusRequest[] model);
    }
}