using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.Player;

namespace TGS.Services.Operator.Services.Player
{
    internal class SetWeChatCommandService : ICommandService<SetWeChatCommand>
    {
        private readonly IPlayerOperations _playerOperations;

        public SetWeChatCommandService(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(SetWeChatCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations
                .SetWeChatAsync(command.PlayerCode, command.WeChatId, updatedOn)
                .ConfigureAwait(false);
        }
    }
}
