using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.TgsTransaction.Core
{
    public interface ITgsTransaction
    {
        Task CreateTransactionPromotionsAsync(TransactionPromotion[] transactionPromotion);
    }
}