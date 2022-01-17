using System;
using System.Linq;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface IGameActionService
    {
        IQueryable<GameAction> GetGameActionQueries(
            Func<IQueryable<GameAction>, IQueryable<GameAction>> preQuery = null,
            bool tracking = false);
    }

    public class GameActionService : BaseRepositoryOperations<ITgsRepository>, IGameActionService
    {
        public GameActionService(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
        }

        public IQueryable<GameAction> GetGameActionQueries(
            Func<IQueryable<GameAction>, IQueryable<GameAction>> preQuery, bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}