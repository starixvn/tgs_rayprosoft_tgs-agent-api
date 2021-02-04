using System;
using System.Linq;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IRoundService
    {
        ITgsRepository Repository { get; }

        IQueryable<Round> GetRoundsAsync(Func<IQueryable<Round>, IQueryable<Round>> preQuery = null,
            bool tracking = false);
    }

    public class RoundService : BaseRepositoryOperations<ITgsRepository>, IRoundService
    {
        public readonly ITgsRepository _repository;

        public RoundService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        ITgsRepository IRoundService.Repository => _repository;

        IQueryable<Round> IRoundService.GetRoundsAsync(
            Func<IQueryable<Round>, IQueryable<Round>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}