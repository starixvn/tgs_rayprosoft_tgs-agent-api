using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class CheckBrandPhoneQueryService : IGetService<CheckBrandPhoneQuery, bool>
{
    private readonly IPlayerServiceClient _playerServiceClient;

    public CheckBrandPhoneQueryService(IPlayerServiceClient playerServiceClient)
    {
        _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
    }

    public async ValueTask<bool> GetAsync(CheckBrandPhoneQuery query, CancellationToken cancellationToken = default)
    {
        var player = await _playerServiceClient.GetPlayerInfoByPhoneAsync(
            query.BrandId,
            query.AreaCode,
            query.Phone,
            cancellationToken).ConfigureAwait(false);

        return player is null || query.PlayerCode is not null && player.PlayerCode == query.PlayerCode;
    }
}
