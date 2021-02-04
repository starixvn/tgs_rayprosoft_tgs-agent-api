using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Queries
{
    public interface IRoundQueries
    {
        Task<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId,
            int gameId);

        Task<Round> GetRoundByRoundIdAsync(Guid roundId, bool includeGameActions = false, bool includeRoundDetail = false);
    }

    public sealed class RoundQueries : IRoundQueries
    {
        private readonly ITgsRepository _repository;

        public RoundQueries(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task<RoundStatus?> IRoundQueries.GetRoundStatusAsync(Guid gameProviderId, string playerCode,
            string externalRoundId,
            int gameId) =>
            _repository.Rounds
                .Where(r => r.GameProviderId == gameProviderId && r.PlayerCode == playerCode &&
                    r.ExternalRoundId == externalRoundId && r.GameId == gameId)
                .Select(r => (RoundStatus?) r.Status)
                .FirstOrDefaultAsync();

        Task<Round> IRoundQueries.GetRoundByRoundIdAsync(Guid roundId, bool includeGameActions, bool includeRoundDetail)
        {
            IQueryable<Round> query = _repository.Rounds.Include(r => r.RoundDetail);

            if (includeGameActions)
            {
                query = query.Include(c => c.GameActions);
            }

            if (includeRoundDetail)
            {
                query = query.Include(c => c.RoundDetail);
            }

            return query.FirstOrDefaultAsync(c => c.Id == roundId);
        }
    }
}