using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Services.Agent.Exceptions;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.Withdraw
{
    internal class CreateWithdrawFormCommandService : ICommandService<CreateWithdrawFormCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IAgentWalletRepository _walletRepository;

        public CreateWithdrawFormCommandService(
            IAgentRepository agentRepository,
            IAgentWalletRepository walletRepository,
            IPaymentServiceClient paymentServiceClient)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _paymentServiceClient =
                paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask ExecuteAsync(CreateWithdrawFormCommand command,
            CancellationToken cancellationToken = default)
        {
            var agentData = await _agentRepository
                .GetAgentByIdAsync(command.AgentId)
                .ConfigureAwait(false);

            var balance = await _walletRepository
                .GetBalanceAsync(agentData.DefaultWalletId, command.AgentId)
                .ConfigureAwait(false);

            if (balance - command.Amount < 0)
                throw new InsufficientFundsException();

            var createFormId = await _paymentServiceClient
                .CreateWithdrawFormAsync(
                    new WithdrawFormCreateInfo
                    {
                        UserId = command.AgentId.ToString(),
                        Amount = command.Amount,
                        BankInfoId = command.BankInfoId,
                        BrandId = command.BrandId,
                        UserName = agentData.Username,
                        UserType = UserType.Agent
                    })
                .ConfigureAwait(false);

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