using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.CashService.Client;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Operations;
using TGS.Services.Operator.BrandPayment;
using TGS.Services.PlayerBucket;
using TGS.Services.Operator.ApplicationServices.Withdraw;

namespace TGS.Services.Operator.ApplicationServices.Withdraw
{
    internal class CreateWithdrawFormCommandService : ICommandService<CreateWithdrawFormCommand>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerOperations _playerOperations;

        public CreateWithdrawFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            ICashServiceClient cashServiceClient,
            IPlayerOperations playerOperations,
            IPlayerBucketRepository playerBucketRepository,
            IJsonSerializationProvider jsonSerializationProvider,
            IOptions<JsonOptions> jsonOptionsAccessor)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
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

            var balance = await _cashServiceClient
                .GetBalanceAsync(playerData.DefaultWalletId)
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

            var result = await _cashServiceClient
                .WithdrawalAsync(
                    playerData.DefaultWalletId,
                    command.Amount,
                    null,
                    null)
                .ConfigureAwait(false);

            if (result.Balance < 0)
            {
                await _cashServiceClient.CancelWithdrawalAsync(
                    playerData.DefaultWalletId,
                    command.Amount,
                    null,
                    null,
                    null).ConfigureAwait(false);

                await _paymentServiceClient
                    .SubmitWithdrawFormStatusAsync(
                        createFormId,
                        PaymentFlowState.Reject)
                    .ConfigureAwait(false);

                command.ReturnFormIdSource.SetResult(createFormId);

                throw new InsufficientFundsException();
            }

            await _paymentServiceClient
                .SubmitWithdrawFormStatusAsync(
                    createFormId,
                    PaymentFlowState.WaitApprove)
                .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
