using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using Valhalla.Services;

namespace TGS.Services.Operator.PlayerPassword
{
    internal class ResetPasswordCommandService : ICommandService<ResetPasswordCommand>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ResetPasswordCommandService(
            IPlayerOperations playerOperations,
            IPasswordHandler passwordHandler,
            IValidateCodeOperator validateCodeOperator)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask ExecuteAsync(ResetPasswordCommand command)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync(command.ValidateCode, command.Token)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var username = await _playerOperations
                .GetUsernameByPlayerCodeAsync(result.PlayerCode)
                .ConfigureAwait(false);
            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);
            await _playerOperations
                .ChangePlayerPasswordAsync(new()
                {
                    Username = username,
                    PlayerCode = result.PlayerCode,
                    Salt = newSalt,
                    Password = newPassword,
                    UpdatedOn = updatedOn
                })
                .ConfigureAwait(false);
        }
    }
}
