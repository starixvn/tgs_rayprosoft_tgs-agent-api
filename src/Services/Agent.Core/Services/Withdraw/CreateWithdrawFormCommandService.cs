using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment;
using TGS.Core.Infrastructures.Json;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Withdraw
{
    internal class CreateWithdrawFormCommandService : ICommandService<CreateWithdrawFormCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAgentWalletRepository _walletRepository;

        public CreateWithdrawFormCommandService(
            IAgentRepository agentRepository,
            IAgentWalletRepository walletRepository,
            IPaymentRepository paymentRepository,
            IJsonSerializationProvider jsonSerializationProvider,
            IOptions<JsonOptions> jsonOptionsAccessor)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _jsonSerializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;
        }

        public async ValueTask ExecuteAsync(CreateWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var agentData = await _agentRepository
                .GetAgentByIdAsync(command.AgentId)
                .ConfigureAwait(false);

            var balance = await _walletRepository
                .GetBalanceAsync(agentData.DefaultWalletId, command.AgentId)
                .ConfigureAwait(false);

            if (balance - command.Amount < 0)
                throw new InsufficientFundsException();

            var createFormId = Guid.NewGuid();

            var result = await _walletRepository
                .ExchangeAsync(
                    command.BrandId,
                    agentData.DefaultWalletId,
                    command.AgentId,
                    TransactionTypes.Withdrawal,
                    -command.Amount)
                .ConfigureAwait(false);

            if (result.PostBalance < 0)
            {
                await _walletRepository.ExchangeAsync(
                    command.BrandId,
                    agentData.DefaultWalletId,
                    command.AgentId,
                    TransactionTypes.Deposit,
                    command.Amount).ConfigureAwait(false);

                await _paymentRepository.CreateWithdrawFormAsync(
                    createFormId,
                    new WithdrawFormCreateInfo(
                        command.AgentId.ToString(),
                        agentData.UserName,
                        UserType.Player,
                        command.BrandId,
                        command.Amount,
                        7000,
                        null,
                        command.BankInfoId)).ConfigureAwait(false);

                command.ReturnFormIdSource.SetResult(createFormId);

                throw new InsufficientFundsException();
            }

            var bankInfo = await _paymentRepository
                .GetAgentBankInfoByIdAsync(command.BankInfoId)
                .ConfigureAwait(false);

            var bankInfoData = new WithdrawBankInfo<JsonDocument>(
                bankInfo.Id,
                bankInfo.PaymentChannelId,
                bankInfo.BankType,
                _jsonSerializationProvider.DeserializeFromString<JsonDocument>(bankInfo.Detail));

            await _paymentRepository
                .CreateWithdrawFormAsync(
                    createFormId,
                new WithdrawFormCreateInfo(
                        command.AgentId.ToString(),
                        agentData.UserName,
                        UserType.Player,
                        command.BrandId,
                        command.Amount,
                        1000,
                        null,
                        command.BankInfoId,
                        BankInfo: _jsonSerializationProvider.SerializeToString(bankInfoData, _jsonSerializerOptions)))
                .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
