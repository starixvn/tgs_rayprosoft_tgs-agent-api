using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.TgsTransaction.Core;

namespace Transaction.Repositories
{
    public class TransactionService : BaseRepositoryOperations<ITgsRepository>, ITgsTransaction
    {
        private readonly ITgsRepository _repository;

        public TransactionService(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
        }

        Task ITgsTransaction.CreateTransactionPromotionsAsync(TransactionPromotion[] transactionPromotions) =>
            CreateEntitiesAsync(transactionPromotions);
    }
}