using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Json;
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
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IWalletRepository _walletRepository;

        public CreateWithdrawFormCommandService(
            IWalletRepository walletRepository,
            IPaymentRepository paymentRepository,
            IPlayerOperations playerOperations,
            IJsonSerializationProvider jsonSerializationProvider,
            IOptions<JsonOptions> jsonOptionsAccessor)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _jsonSerializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;
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

            var createFormId = Guid.NewGuid();

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

                await _paymentRepository.CreateWithdrawFormAsync(
                    createFormId,
                    new WithdrawFormInfo(
                        command.PlayerCode,
                        command.BrandId,
                        command.Amount,
                        7000,
                        command.BankInfoId)).ConfigureAwait(false);

                command.ReturnFormIdSource.SetResult(createFormId);

                throw new InsufficientFundsException();
            }

            var bankInfo = await _paymentRepository
                .GetBankInfoByIdAsync(command.BankInfoId)
                .ConfigureAwait(false);

            await _paymentRepository
                .CreateWithdrawFormAsync(
                    createFormId,
                new WithdrawFormInfo(
                        command.PlayerCode,
                        command.BrandId,
                        command.Amount,
                        1000,
                        command.BankInfoId,
                        BankInfo: _jsonSerializationProvider.SerializeToString(bankInfo, _jsonSerializerOptions)))
                .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
