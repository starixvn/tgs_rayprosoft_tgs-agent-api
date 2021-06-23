using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Operations;
using TGS.Services.Operator.BrandPayment;
using TGS.Services.PlayerBucket;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Withdraw
{
    internal class CreateWithdrawFormCommandService : ICommandService<CreateWithdrawFormCommand>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IWalletRepository _walletRepository;

        public CreateWithdrawFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IWalletRepository walletRepository,
            IPlayerOperations playerOperations,
            IPlayerBucketRepository playerBucketRepository,
            IJsonSerializationProvider jsonSerializationProvider,
            IOptions<JsonOptions> jsonOptionsAccessor)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _jsonSerializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;
        }

        public async ValueTask ExecuteAsync(CreateWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var isBucketFull = await _playerBucketRepository
                .IsBucketFullAsync(command.PlayerCode)
                .ConfigureAwait(false);

            if (!isBucketFull)
                throw new InsufficientBucketException();

            var playerData = await _playerOperations
                .GetCachedPlayerAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var balance = await _walletRepository
                .GetBalanceAsync(playerData.DefaultWalletId, command.PlayerCode)
                .ConfigureAwait(false);

            if (balance - command.Amount < 0)
                throw new InsufficientFundsException();

            var createFormId = await _paymentServiceClient
                .CreateWithdrawFormAsync(
                    new WithdrawFormCreateInfo
                    {
                        Amount = command.Amount,
                        BrandId = command.BrandId,
                        BankInfoId = command.BankInfoId,
                        UserId = command.PlayerCode,
                        UserName = playerData.Username,
                        UserType = UserType.Player
                    })
                .ConfigureAwait(false);

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

                await _paymentServiceClient
                    .SubmitWithdrawFormStatusAsync(
                        createFormId,
                        PaymentFlowState.Reject)
                    .ConfigureAwait(false);

                command.ReturnFormIdSource.SetResult(createFormId);

                throw new InsufficientFundsException();
            }

            await _paymentServiceClient
                .SubmitDepositFormStatusAsync(
                    createFormId,
                    PaymentFlowState.WaitApprove)
                .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
