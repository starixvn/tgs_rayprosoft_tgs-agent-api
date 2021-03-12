using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using Valhalla.Services;

namespace TGS.Services.Operator.PayPassowrd
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

        public async ValueTask ExecuteAsync(SetFirstPayPasswordCommand command)
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
