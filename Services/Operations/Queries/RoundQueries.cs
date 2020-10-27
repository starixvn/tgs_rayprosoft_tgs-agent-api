using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Bet;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Queries
{
    public interface IRoundQueries
    {
        Task<RoundStatus?> GetRoundStatusAsync(Guid gameProviderId, string externalRoundId);
    }

    public sealed class RoundQueries : IRoundQueries
    {
        private readonly IBetRepository _repository;

        public RoundQueries(IBetRepository repository)
        {
            _repository = repository;
        }

        Task<RoundStatus?> IRoundQueries.GetRoundStatusAsync(Guid gameProviderId, string externalRoundId)
        {
            return _repository.Rounds
                .Where(r => r.GameProviderId == gameProviderId && r.ExternalRoundId == externalRoundId)
                .Select(r => (RoundStatus?) r.Status)
                .FirstOrDefaultAsync();
        }
    }
}
