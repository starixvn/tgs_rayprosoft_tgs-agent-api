using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.BankInfos;

internal class CreateBankInfoCommandService(
    IPaymentServiceClient paymentServiceClient,
    IAgentRepository agentRepository)
    : ICommandService<CreateBankInfoCommand>
{
    private readonly IAgentRepository _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    private readonly IPaymentServiceClient _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));

    public async ValueTask ExecuteAsync(CreateBankInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository
            .GetAgentByIdAsync(command.AgentId)
            .ConfigureAwait(false);

        var createInfo = new BankCreateInfo
        {
            PaymentChannelId = command.PaymentChannelId,
            BankType = command.BankType,
            Detail = JsonDocument.Parse(command.Detail)
        };

        await _paymentServiceClient
            .CreateAgentBankInfoAsync(
                command.BrandId,
                command.AgentId,
                agent!.Username,
                createInfo)
            .ConfigureAwait(false);
    }
}
