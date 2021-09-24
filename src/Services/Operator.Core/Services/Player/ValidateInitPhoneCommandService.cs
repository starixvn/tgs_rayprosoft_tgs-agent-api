using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Exceptions;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    public class ValidateInitPhoneCommandService : IGetService<ValidateInitPhoneCommand, string>
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerRepository _playerRepository;

        public ValidateInitPhoneCommandService(
            IPlayerRepository playerRepository,
            IPasswordHandler passwordHandler)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask<string> GetAsync(ValidateInitPhoneCommand command,
            CancellationToken cancellationToken = default)
        {
            var playerPasswordInfo = await _playerRepository
                .GetPlayerPasswordInfoByUsernameAsync(command.BrandId, command.Username).ConfigureAwait(false);

            if (playerPasswordInfo == null)
            {
                throw new PlayerNotFoundException("");
            }

            if (!_passwordHandler.Verify(playerPasswordInfo.Password, playerPasswordInfo.Salt, command.Password))
            {
                throw new InvalidCredentialsException("");
            }

            return playerPasswordInfo.PlayerCode;
        }
    }
}