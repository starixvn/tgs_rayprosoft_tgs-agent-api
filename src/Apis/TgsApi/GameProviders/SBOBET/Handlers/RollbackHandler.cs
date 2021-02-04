using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    /// <summary>
    /// case: Cancel Settle or Cancel CancelTransaction
    /// </summary>
    public sealed class RollbackHandler
        : IRequestHandler<RollbackRequest, RollbackResponse>
    {
        private static readonly GameActionType[] ActionTypes = {WinBet, LoseBet, TieBet, FreeBet, CancelTransaction};

        private readonly IGamePlayActions _actions;
        private readonly ISBOBETGameControllerMapper _map;
        private readonly ISBOBETServices _services;

        public RollbackHandler(
            IGamePlayActions actions,
            ISBOBETGameControllerMapper map,
            ISBOBETServices services)
        {
            _actions = actions;
            _map = map;
            _services = services;
        }

        async Task<RollbackResponse> IRequestHandler<RollbackRequest, RollbackResponse>.Handle(RollbackRequest request,
            CancellationToken cancellationToken)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();
            var externalRoundId = request.ExternalTransactionId;

            var gameProviderId = data.GameProviderId;
            var gameId = await _services.GetGameIdAsync(gameProviderId, request.ExternalGameId, data.PlatformType);

            var gameActionExternalIds =
                await _services.GetGameActionExternalIdsAsync(gameProviderId, request.PlayerCode, externalRoundId,
                    gameId,
                    ActionTypes);

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }
    }
}