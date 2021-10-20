using System;
using System.Linq;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITransactionService
    {
        ITgsRepository TgsRepository { get; }

        IQueryable<Transaction> GetTransactionQueries(
            Func<IQueryable<Transaction>, IQueryable<Transaction>> preQuery = null,
            bool tracking = false);
    }

    public class TransactionService : BaseRepositoryOperations<ITgsRepository>, ITransactionService
    {
        public readonly ITgsRepository _repository;

        public TransactionService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository ITransactionService.TgsRepository => _repository;

        IQueryable<Transaction> ITransactionService.GetTransactionQueries(
            Func<IQueryable<Transaction>, IQueryable<Transaction>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}