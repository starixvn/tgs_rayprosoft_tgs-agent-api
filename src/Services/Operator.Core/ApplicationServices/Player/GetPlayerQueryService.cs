using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class GetPlayerQueryService : IGetService<GetPlayerQuery, PlayerInfo>
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerQueryService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    }

    public async ValueTask<PlayerInfo> GetAsync(
        GetPlayerQuery query,
        CancellationToken cancellationToken = default)
    {
        var player = await _playerRepository
        .GetPlayerInfoAsync(query.PlayerCode)
        .ConfigureAwait(false);

        if (player == null)
        {
            throw new PlayerNotFoundException("Player not found.");
        }

        return player;
    }
}
