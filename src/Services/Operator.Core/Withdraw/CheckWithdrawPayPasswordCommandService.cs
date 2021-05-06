using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Services.Operations.Queries;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Withdraw
{
    internal class CheckWithdrawPayPasswordCommandService : ICommandService<CheckWithdrawPayPasswordCommand>
    {
        private readonly IPasswordHandler _passwordHandler;
        private readonly IPlayerQueries _playerQueries;

        public CheckWithdrawPayPasswordCommandService(IPlayerQueries playerQueries, IPasswordHandler passwordHandler)
        {
            _playerQueries = playerQueries ?? throw new ArgumentNullException(nameof(playerQueries));
            _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        }

        public async ValueTask ExecuteAsync(CheckWithdrawPayPasswordCommand command, CancellationToken cancellationToken = default)
        {
            var (paySalt, payPassword) = await _playerQueries
                .GetPaySaltAndPasswordByPlayerCodeAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var isVaild = _passwordHandler.Verify(payPassword, paySalt, command.PayPassword);

            command.ReturnSource.SetResult(isVaild);
        }
    }
}
