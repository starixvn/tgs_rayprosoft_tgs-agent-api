using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Operator.ValidateCode;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class CommitSetPlayerPhoneCommandService : ICommandService<CommitSetPlayerPhoneCommand>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidateCodeOperator _validateCodeOperator;

        public CommitSetPlayerPhoneCommandService(
            IValidateCodeOperator validateCodeOperator,
            IPlayerOperations playerOperations)
        {
            _validateCodeOperator =
                validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(CommitSetPlayerPhoneCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<PhoneValidateData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            await _playerOperations
                .SetPhoneAsync(command.PlayerCode, result.Phone, updatedOn)
                .ConfigureAwait(false);
        }
    }
}