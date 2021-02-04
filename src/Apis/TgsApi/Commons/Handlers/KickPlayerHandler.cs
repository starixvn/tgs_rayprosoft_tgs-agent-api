using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.Commons.Handlers
{
    public sealed class KickPlayerHandler : IRequestHandler<PlayerKickRequest, PlayerKickResponse>
    {
        private readonly Func<string, IPlayerAction> _factory;
        private readonly IGameProviderOperations _gameProviderOperations;

        public KickPlayerHandler(
            IGameProviderOperations gameProviderOperations,
            Func<string, IPlayerAction> factory)
        {
            _gameProviderOperations = gameProviderOperations;
            _factory = factory;
        }

        async Task<PlayerKickResponse> IRequestHandler<PlayerKickRequest, PlayerKickResponse>.Handle(
            PlayerKickRequest request, CancellationToken cancellationToken)
        {
            var gameProviderCode = request.GameProviderCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);
            var allowKickPlayer = await _gameProviderOperations.GetAllowKickPlayerAsync(gameProviderId);

            if (!allowKickPlayer)
            {
                throw new NotSupportedException(
                    $"Game Provider '{request.GameProviderCode}' doesn't support to kick player");
            }

            return await _factory(gameProviderCode).KickAsync(request);
        }
    }
}