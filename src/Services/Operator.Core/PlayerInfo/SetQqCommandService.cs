using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.PlayerInfo
{
    class SetQqCommandService : ICommandService<SetQqCommand>
    {
        private readonly IPlayerOperations _playerOperations;

        public SetQqCommandService(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }
        public async ValueTask ExecuteAsync(SetQqCommand command)
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
