using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    internal class ReviewDepositFormCommandService : ICommandService<ReviewDepositFormCommand>
    {
        private readonly IDepositRepository _depositRepository;

        public ReviewDepositFormCommandService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public ValueTask ExecuteAsync(ReviewDepositFormCommand command, CancellationToken cancellationToken = default)
            => _depositRepository.SignOffDepositFormAsync(command.FormId, command.Status, command.remark);
    }
}
