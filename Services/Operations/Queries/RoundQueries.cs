using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Queries
{
    public interface IRoundQueries
    {
        Task<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string playerCode, string externalRoundId,
            int gameId);
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
    }
}