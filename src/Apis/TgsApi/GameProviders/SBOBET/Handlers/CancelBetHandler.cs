using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.SBOBET.Enums;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    /// <summary>
    /// case: Cancel Bet or Void Round
    /// </summary>
    public sealed class CancelBetHandler : IRequestHandler<CancelBetRequest, CancelBetResponse>
    {
        private readonly IGamePlayActions _actions;
        private readonly ISBOBETGameControllerMapper _map;
        private readonly ISBOBETServices _services;

        public CancelBetHandler(
            IGamePlayActions actions,
            ISBOBETGameControllerMapper map,
            ISBOBETServices services)
        {
            _actions = actions;
            _map = map;
            _services = services;
        }

        async Task<CancelBetResponse> IRequestHandler<CancelBetRequest, CancelBetResponse>.Handle(CancelBetRequest request, CancellationToken cancellationToken)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();
            Func<GameProviderConfigurationData, CancelBetRequest, Task<CancelBetResponse>> func =
                request.IsCancelAll ? VoidRoundAsync : CancelBetAsync;

            return await func(data, request);
        }

        
        #region Private Methods

        private async Task<CancelBetResponse> CancelBetAsync(GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var betData = _map.Request(data, request);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        private async Task<CancelBetResponse> VoidRoundAsync(GameProviderConfigurationData data,
            CancelBetRequest request)
        {
            var gameProviderId = data.GameProviderId;
            var externalRoundId = request.ExternalRoundId;

            var playerCode = request.PlayerCode;
            var gameId = await _services.GetGameIdAsync(data.GameProviderId, request.ExternalGameId, data.PlatformType);

            var roundStatus =
                await _services.GetRoundStatusAsync(gameProviderId, playerCode, externalRoundId, gameId);

            var gameActionExternalIds =
                await _services.GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId);

            if (roundStatus == RoundStatus.Closed && gameActionExternalIds.Length == 0)
            {
                return _map.Response<CancelBetResponse>(request.PlayerCode, GameApiError.BetAlreadyCanceled);
            }

            var betData = _map.Request(data, request, gameActionExternalIds);
            var r = await _actions.CancelTransactionAsync(betData);
            return _map.Response(request, r);
        }

        #endregion Private Methods
    }
}
