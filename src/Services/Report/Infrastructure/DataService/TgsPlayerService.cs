using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsPlayerService
    {
        Task<Player> GetPlayerAsync(Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>> preQuery = null);

        IQueryable<Player> GetPlayerQueries(Func<IQueryable<Player>, IQueryable<Player>> preQuery = null,
            bool tracking = false);
    }

    public class TgsPlayerService : BaseRepositoryOperations<ITgsRepository>, ITgsPlayerService
    {
        public TgsPlayerService(ITgsRepository repository) : base(repository)
        {
        }

        Task<Player> ITgsPlayerService.GetPlayerAsync(Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>> preQuery) => GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Player> ITgsPlayerService.GetPlayerQueries(
            Func<IQueryable<Player>, IQueryable<Player>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}