using System;
using System.Threading.Tasks;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Models.Interfaces
{
    public interface IPlayerAction
    {
        ValueTask<PlayerKickResponse> KickAsync(PlayerKickRequest request) =>
            throw new NotSupportedException(
                $"Game Provider '{request.GameProviderCode}' doesn't support to kick player");

        ValueTask<PlayerUpdateSportBetLimitsResponse>
            UpdateSportBetLimitsAsync(PlayerUpdateSportBetLimitsRequest request) =>
            throw new NotSupportedException(
                $"Game Provider '{request.GameProviderCode}' doesn't support to update player sports bet limits");
    }
}