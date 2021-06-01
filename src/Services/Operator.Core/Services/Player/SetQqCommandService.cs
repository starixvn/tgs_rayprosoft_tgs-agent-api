using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Player;

namespace TGS.Services.Operator.Services.Player
{
    internal class SetQqCommandService : ICommandService<SetQqCommand>
    {
        private readonly IPlayerOperations _playerOperations;

        public SetQqCommandService(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(SetQqCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations
                .SetQqAsync(command.PlayerCode, command.QqId, updatedOn)
                .ConfigureAwait(false);
        }
    }
}
