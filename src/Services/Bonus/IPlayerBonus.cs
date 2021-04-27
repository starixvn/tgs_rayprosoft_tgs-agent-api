using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;

namespace TGS.Services.Bonus
{
    public interface IPlayerBonus
    {
        ITgsRepository Repository { get; }
        Task SentPromotionBonus(IEnumerable<PromotionPlayerBonus> playerBonus, TransactionType transactionType);
        Task SentBonus(IEnumerable<PlayerRebates> playerBonus, TransactionType transactionType);
    }
}