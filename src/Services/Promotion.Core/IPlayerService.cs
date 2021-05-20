using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Promotion
{
    public interface IPlayerService
    {
        Task<bool> ExistsAsync(Expression<Func<Player, bool>> predicate);

        Task<Player> GetPlayerAsync(
            Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>>? preQuery = null);

        Task<Player?> GetPlayerByPlayerCodeAsync(string playerCode);

        IQueryable<PlayerAuditLog> GetPlayerOperationQueries(
            Func<IQueryable<PlayerAuditLog>, IQueryable<PlayerAuditLog>>? preQuery = null,
            bool tracking = false);

        IQueryable<Player> GetPlayerQueries(
            Func<IQueryable<Player>, IQueryable<Player>>? preQuery = null,
            bool tracking = false);

        Task UpdatePlayerAsync(Player player, UpdatePlayerRequest request);
    }
}
