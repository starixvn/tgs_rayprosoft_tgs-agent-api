using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class GetPlayerIdentityFormStatusQueryService : IGetService<GetPlayerIdentityFormStatusQuery, PlayerIdentityReviewState>
    {
        private readonly IPlayerRepository _playerRepository;

        public GetPlayerIdentityFormStatusQueryService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<PlayerIdentityReviewState> GetAsync(GetPlayerIdentityFormStatusQuery query, CancellationToken cancellationToken = default) 
            => query.IdentityAuthentication
                ? PlayerIdentityReviewState.Approve
                : await _playerRepository
                .GetPlayerIdentityFormStatusQueryAsync(query.playerCode)
                .ConfigureAwait(false);
    }
}
