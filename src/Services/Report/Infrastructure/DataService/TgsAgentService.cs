using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsAgentService
    {
        Task<Agent> GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null);

        IQueryable<Agent> GetAgentQueries(Func<IQueryable<Agent>, IQueryable<Agent>> preQuery = null,
            bool tracking = false);

        IQueryable<AgentTransaction> GetAgentTransactionQueries(
            Func<IQueryable<AgentTransaction>, IQueryable<AgentTransaction>> preQuery = null,
            bool tracking = false);
    }

    public class TgsAgentService : BaseRepositoryOperations<ITgsRepository>, ITgsAgentService
    {
        private readonly ITgsRepository _repository;

        public TgsAgentService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        Task<Agent> ITgsAgentService.GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery) => GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Agent> ITgsAgentService.GetAgentQueries(
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        IQueryable<AgentTransaction> ITgsAgentService.GetAgentTransactionQueries(
            Func<IQueryable<AgentTransaction>, IQueryable<AgentTransaction>> preQuery, bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}