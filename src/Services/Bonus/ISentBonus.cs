using System.Threading.Tasks;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;

namespace Bonus
{
    public interface ISentBonus
    {
        Task Sent(PlayerBonus[] playerBonus, TransactionType transactionType, bool isPromotion);
    }
}