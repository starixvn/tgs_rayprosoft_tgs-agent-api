using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class RemoveFavoriteGameCommandService : ICommandService<RemoveFavoriteGameCommand>
    {
        private readonly IGameProviderRepository _gameProviderRepository;

        public RemoveFavoriteGameCommandService(IGameProviderRepository gameProviderRepository)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public ValueTask ExecuteAsync(RemoveFavoriteGameCommand command, CancellationToken cancellationToken = default)
            => _gameProviderRepository.RemoveFavoriteGameAsync(command.PlayerCode, command.GameId);
    }
}
