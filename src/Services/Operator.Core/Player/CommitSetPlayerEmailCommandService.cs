using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Player
{
    internal class CommitSetPlayerEmailCommandService : ICommandService<CommitSetPlayerEmailCommand>
    {
        private readonly IValidateCodeOperator _validateCodeOperator;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITgsRepository _tgsRepository;

        public CommitSetPlayerEmailCommandService(
            IValidateCodeOperator validateCodeOperator,
            IPlayerOperations playerOperations,
            ITgsRepository tgsRepository)
        {
            _validateCodeOperator = validateCodeOperator ?? throw new ArgumentNullException(nameof(validateCodeOperator));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask ExecuteAsync(CommitSetPlayerEmailCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeOperator
                .GetValidateCodeDataAsync<EmailValidateData>(command.ValidateCode, command.SecurityKey)
                .ConfigureAwait(false);

            if (result == null)
                throw new ValidateCodeNotVerifyException();

            var updatedOn = DateTimeOffset.UtcNow;

            var isMailDuplicate = _tgsRepository.Players.AsQueryable()
                    .Any(player => player.Email == result.Email);

            if (isMailDuplicate)
                throw new EmailAlreadySetException();

            await _playerOperations
                .SetEmailAsync(command.PlayerCode, result.Email, updatedOn)
                .ConfigureAwait(false);
        }
    }
}