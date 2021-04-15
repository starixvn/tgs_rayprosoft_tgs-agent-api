using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Player
{
    internal class CommitSetPlayerEmailCommandService : ICommandService<CommitSetPlayerEmailCommand>
    {
        private readonly IValidateCodeOperator _validateCodeOperator;
        private readonly IPlayerOperations _playerOperations;

        public CommitSetPlayerEmailCommandService(
            IValidateCodeOperator validateCodeOperator,
            IPlayerOperations playerOperations)
        {
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(CommitSetPlayerEmailCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            await _playerOperations
                .SetEmailAsync(command.PlayerCode, result.Email, updatedOn)
                .ConfigureAwait(false);
        }
    }
}
