using System;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.PlayerIdentity
{
    internal class SetIdentityCommandService : ICommandService<SetIdentityCommand>
    {
        private readonly IPlayerOperations _playerOperations;

        public SetIdentityCommandService(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(SetIdentityCommand command)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var updatedOn = DateTimeOffset.UtcNow;
            await _playerOperations
                .SetIdentityAsync(
                    command.PlayerCode,
                    command.RealName,
                    command.Identity,
                    player.DefaultWalletCurrencyCode == "CNY" ? true : false,
                    updatedOn)
                .ConfigureAwait(false);
        }
    }
}
