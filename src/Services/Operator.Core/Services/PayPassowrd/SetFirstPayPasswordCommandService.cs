using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.PayPassowrd;

namespace TGS.Services.Operator.Services.PayPassowrd
{
    internal class SetFirstPayPasswordCommandService : ICommandService<SetFirstPayPasswordCommand>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IPasswordHandler _passwordHandler;

        public SetFirstPayPasswordCommandService(IPlayerOperations playerOperations, IPasswordHandler passwordHandler)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler;
        }

        public async ValueTask ExecuteAsync(SetFirstPayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);
            var updatedOn = DateTimeOffset.UtcNow;

            if (player.WithdrawPassword.Length > 0)
                throw new PayPasswordAlreadySetException();

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.Password);
            await _playerOperations.SetPayPasswordAsync(command.PlayerCode, newPassword, newSalt, updatedOn);
        }
    }
}
