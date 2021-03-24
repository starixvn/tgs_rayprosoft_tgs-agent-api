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
    }

    public class TgsAgentService : BaseRepositoryOperations<ITgsRepository>, ITgsAgentService
    {
        public TgsAgentService(ITgsRepository repository) : base(repository)
        {
        }

        Task<Agent> ITgsAgentService.GetAgentAsync(Expression<Func<Agent, bool>> predicate,
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery) => GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Agent> ITgsAgentService.GetAgentQueries(
            Func<IQueryable<Agent>, IQueryable<Agent>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}