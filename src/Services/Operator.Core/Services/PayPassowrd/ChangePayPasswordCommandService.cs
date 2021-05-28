using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations;
using TGS.ServiceUtility;
using TGS.Services.Operator.Services.PayPassowrd;
using TGS.Services.Operator.ValidateCode;

namespace TGS.Services.Operator.Services.PayPassowrd
{
    internal class ChangePayPasswordCommandService : ICommandService<ChangePayPasswordCommand>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public ChangePayPasswordCommandService(
            IPlayerOperations playerOperations,
            IPasswordHandler passwordHandler,
            IValidateCodeOperator validateCodeOperator)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
        }

        public async ValueTask ExecuteAsync(ChangePayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var player = await _playerOperations
                .GetPlayerByCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<ValidateCodeData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            var (newSalt, newPassword) = _passwordHandler.GenerateCredential(command.NewPassword);
            await _playerOperations.SetPayPasswordAsync(command.PlayerCode, newPassword, newSalt, updatedOn);
        }
    }
}
