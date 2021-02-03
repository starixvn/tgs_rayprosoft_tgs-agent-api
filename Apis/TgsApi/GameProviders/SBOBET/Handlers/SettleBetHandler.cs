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
    public sealed class SettleBetHandler : IRequestHandler<SettleBetRequest, SettleBetResponse>
    {
        private readonly IGamePlayActions _actions;
        private readonly ISBOBETGameControllerMapper _map;
        private readonly ISBOBETServices _services;

        private static readonly GameActionType[] ActionTypes = {WinBet, LoseBet, TieBet, FreeBet};

        public SettleBetHandler(
            IGamePlayActions actions,
            ISBOBETGameControllerMapper map,
            ISBOBETServices services)
        {
            _actions = actions;
            _map = map;
            _services = services;
        }

        async Task<SettleBetResponse> IRequestHandler<SettleBetRequest, SettleBetResponse>.Handle(SettleBetRequest request, CancellationToken cancellationToken)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();
            var gameProviderId = data.GameProviderId;
            var externalRoundId = request.ExternalTransactionId;

            var playerCode = request.PlayerCode;
            var gameId = await _services.GetGameIdAsync(gameProviderId, request.ExternalGameId, data.PlatformType);

            var roundStatus = await _services.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);
            var gameActionExternalIds = await _services.GetGameActionExternalIdsAsync(gameProviderId,
                playerCode, externalRoundId, gameId, ActionTypes);

            var betData = _map.Request(data, externalRoundId, request, roundStatus, gameActionExternalIds);
            var r = await _actions.SettleBetAsync(betData);
            return _map.Response(request, r);
        }
    }
}
