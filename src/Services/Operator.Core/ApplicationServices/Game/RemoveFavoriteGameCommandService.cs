using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operator.GameProviders;
using Tgs.ServiceUtility;
using TGS.Services.Operator.ApplicationServices.Game;

namespace TGS.Services.Operator.ApplicationServices.Game;

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
