using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    public class GetPlayerInfoByUserNameQueryService : IGetService<GetPlayerInfoByUserNameQuery, PlayerInfo?>
    {
        private readonly IPlayerRepository _playerRepository;

        public GetPlayerInfoByUserNameQueryService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public ValueTask<PlayerInfo?> GetAsync(
            GetPlayerInfoByUserNameQuery query,
            CancellationToken cancellationToken = default)
            => _playerRepository.GetPlayerInfoByUserNameAsync(query.BrandId, query.UserName);
    }
}
