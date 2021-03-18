using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Services;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Shared.Game.Models.SBOBET.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Handlers
{
    public sealed class GetBetStatusHandler : IRequestHandler<GetBetStatusRequest, GetBetStatusResponse>
    {
        private readonly ISBOBETGameControllerMapper _map;
        private readonly ISBOBETServices _services;

        public GetBetStatusHandler(
            ISBOBETGameControllerMapper map,
            ISBOBETServices services)
        {
            _map = map;
            _services = services;
        }

        async Task<GetBetStatusResponse> IRequestHandler<GetBetStatusRequest, GetBetStatusResponse>.Handle(
            GetBetStatusRequest request, CancellationToken cancellationToken)
        {
            var data = await _services.GetGameProviderConfigurationDataAsync();
            var externalRoundId = request.ExternalRoundId;
            var playerCode = request.PlayerCode;
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

            var ga = await _services.GetValidGameActionsAsync(gameProviderId, request.PlayerCode,
                request.ExternalRoundId, gameId.Value);

            if (ga == null)
            {
                throw new InvalidGameActionException(
                    $"Transaction of transferCode: {request.ExternalTransactionId} and transactionId: {request.ExternalRoundId} is not found.",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return _map.Response(request, ga);
        }
    }
}