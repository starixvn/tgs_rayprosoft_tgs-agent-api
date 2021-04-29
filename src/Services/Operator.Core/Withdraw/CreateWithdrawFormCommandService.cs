using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Payment;
using TGS.Services.Wallet;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Withdraw
{
    internal class CreateWithdrawFormCommandService : ICommandService<CreateWithdrawFormCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPlayerOperations _playerOperations;
        private readonly IWalletRepository _walletRepository;

        public CreateWithdrawFormCommandService(
            IWalletRepository walletRepository,
            IPaymentRepository paymentRepository,
            IPlayerOperations playerOperations)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask ExecuteAsync(CreateWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var playerData = await _playerOperations
                .GetCachedPlayerAsync(command.PlayerCode)
                .ConfigureAwait(false);
            var balance = await _walletRepository
                .GetBalanceAsync(playerData.DefaultWalletId, command.PlayerCode)
                .ConfigureAwait(false);

            if (balance - command.Amount < 0)
                throw new InsufficientFundsException();

            var result = await _walletRepository
                .ExchangeAsync(
                    command.BrandId,
                    playerData.DefaultWalletId,
                    command.PlayerCode,
                    TransactionTypes.Withdrawal,
                    -command.Amount)
                .ConfigureAwait(false);

            if (result.PostBalance < 0)
            {
                await _walletRepository.ExchangeAsync(
                    command.BrandId,
                    playerData.DefaultWalletId,
                    command.PlayerCode,
                    TransactionTypes.Deposit,
                    command.Amount).ConfigureAwait(false);



                throw new InsufficientFundsException();
            }
        }
    }
}
