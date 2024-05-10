using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.CashService.Client;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;
using TGS.Services.Agent.Exceptions;

namespace TGS.Services.Agent.ApplicationServices.Withdraw;

internal class CreateWithdrawFormCommandService(
    IPaymentServiceClient paymentServiceClient,
    ICashServiceClient cashServiceClient,
    IAgentClient agentClient)
    : ICommandService<CreateWithdrawFormCommand>
{
    private readonly IPaymentServiceClient _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));

    public async ValueTask ExecuteAsync(CreateWithdrawFormCommand command,
        CancellationToken cancellationToken = default)
    {
        var agent = await agentClient.GetAgentInfoAsync(new GetAgentInfoRequest(command.AgentId), cancellationToken)
            .ConfigureAwait(false);

        var balance = await cashServiceClient.GetBalanceAsync(agent!.WalletId!.Value, cancellationToken)
            .ConfigureAwait(false);

        if (balance - command.Amount < 0)
            throw new Exceptions.InsufficientFundsException();

        var createFormId = await _paymentServiceClient
            .CreateWithdrawFormAsync(
                new WithdrawFormCreateInfo
                {
                    UserId = command.AgentId.ToString(),
                    Amount = command.Amount,
                    BankInfoId = command.BankInfoId,
                    BrandId = command.BrandId,
                    UserName = agent.Username,
                    UserType = UserType.Agent
                }, cancellationToken)
            .ConfigureAwait(false);

        await _paymentServiceClient
            .SubmitWithdrawFormStatusAsync(
                createFormId,
                PaymentFlowState.WaitApprove,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        command.ReturnFormIdSource.SetResult(createFormId);
    }
}
