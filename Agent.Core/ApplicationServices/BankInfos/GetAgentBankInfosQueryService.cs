using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Agent.ApplicationServices.BankInfos;

internal class GetAgentBankInfosQueryService(IPaymentServiceClient paymentServiceClient)
    : IQueryService<GetAgentBankInfosQuery, AgentBankInfo>
{
    private readonly IPaymentServiceClient _paymentServiceClient =
        paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));

    public IAsyncEnumerable<AgentBankInfo> QueryAsync(
        GetAgentBankInfosQuery query,
        CancellationToken cancellationToken = default)
        => _paymentServiceClient.GetAgentBankInfosAsync(query.AgentId);
}