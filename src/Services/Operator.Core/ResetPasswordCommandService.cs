using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using Valhalla.Services;

namespace TGS.Services.Operator
{
    internal class ResetPasswordCommandService : ICommandService<ResetPasswordCommand>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IPasswordHandler _passwordHandler;

        public ResetPasswordCommandService(IPlayerOperations playerOperations, IPasswordHandler passwordHandler)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask ExecuteAsync(ResetPasswordCommand command)
        {
            var username = await _playerOperations.GetUsernameByPlayerCodeAsync(command.PlayerCode);
            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);
            await _playerOperations.ChangePlayerPasswordAsync(new()
            {
                Username = username,
                PlayerCode = command.PlayerCode,
                Salt = newSalt,
                Password = newPassword,
                UpdatedOn = updatedOn
            });
        }
    }
}
