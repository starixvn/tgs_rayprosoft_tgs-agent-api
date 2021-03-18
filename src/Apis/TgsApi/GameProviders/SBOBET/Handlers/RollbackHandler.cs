using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    /// <summary>
    ///     case: Cancel Settle or Cancel CancelTransaction
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
            var playerCode = request.PlayerCode;
            var externalRoundId = request.ExternalTransactionId;
            var gameProviderId = data.GameProviderId;

            int? gameId;
            if (request.GameType > 0)
            {
                gameId = await _services.GetGameIdAsync(gameProviderId, request.ExternalGameId, data.PlatformType);
            }
            else
            {
                var externalTransactionId = externalRoundId;
                (_, gameId) = await _services.GetRoundStatusWithGameIdAsync(gameProviderId, playerCode,
                    externalRoundId, externalTransactionId);

                if (!gameId.HasValue)
                {
                    throw new InvalidGameActionException(
                        $"Game with referenced round is not found. RoundId: {externalRoundId}",
                        InvalidGameActionErrorCode.GameNotFound);
                }
            }

            var gameActionExternalIds =
                await _services.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId,
                    gameId.Value, ActionTypes);

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }
    }
}