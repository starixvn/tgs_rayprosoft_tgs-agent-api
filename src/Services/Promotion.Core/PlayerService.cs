using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Services.Promotion
{
    public class PlayerService : BaseRepositoryOperations<ITgsRepository>, IPlayerService
    {
        private readonly ITgsRepository _repository;

        public PlayerService(
            ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
            _repository = tgsRepository;
        }

        public Task<bool> ExistsAsync(Expression<Func<Player, bool>> predicate) =>
            base.ExistsAsync(predicate);

        public Task<Player> GetPlayerAsync(
            Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>>? preQuery = null) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        public Task<Player> GetPlayerByPlayerCodeAsync(string playerCode)
            => _repository.Players.AsQueryable()
                .Where(player => player.PlayerCode == playerCode)
                .FirstOrDefaultAsync();

        public IQueryable<Player> GetPlayerQueries(
            Func<IQueryable<Player>, IQueryable<Player>>? preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);

        public async Task SetFirstDepositTimeAsync(string playerCode, DateTimeOffset time)
        {
            var player = await _repository.Players
                .FindAsync(playerCode)
                .ConfigureAwait(false);

            if (player.FirstDepositOn is null)
            {
                player.FirstDepositOn = time;

                await _repository.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}