using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.OperatorApi.Handlers.Player
{
    public sealed class VerifyIdHandler : IRequestHandler<VerifyIdRequest, VerifyIdResponse>
    {
        private readonly IPlayerCommands _playerCommands;
        private readonly IPlayerQueries _playerQueries;
        private readonly ITokenProvider _provider;

        public VerifyIdHandler(
            IPlayerQueries playerQueries,
            IPlayerCommands playerCommands,
            ITokenProvider provider)
        {
            _playerQueries = playerQueries;
            _playerCommands = playerCommands;
            _provider = provider;
        }

        async Task<VerifyIdResponse> IRequestHandler<VerifyIdRequest, VerifyIdResponse>.Handle(
            VerifyIdRequest request, CancellationToken cancellationToken)
        {
            var player = await _playerQueries.GetPlayerByCodeAsync(request.PlayerCode);

            if (player == null)
            {
                throw new PlayerNotFoundException(request.PlayerCode);
            }

            if (player.IdNumber.IsNotNullOrEmpty())
            {
                throw new PlayerIdAlreadyValidatedException();
            }

            var isValidated = request.CurrencyCode switch
            {
                "CNY" => request.Id.CNYIdVerify(),
                _     => false
            };

            if (isValidated)
            {
                player.IdNumber = await _provider.EncryptAsync(request.Id);
                await _playerCommands.UpdatePlayerIdNumberAsync(player);
            }

            return new() {Validated = isValidated};
        }
    }
}