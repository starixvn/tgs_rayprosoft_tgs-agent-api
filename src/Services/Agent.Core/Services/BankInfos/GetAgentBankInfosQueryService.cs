using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.BankInfos
{
    internal class GetAgentBankInfosQueryService : IQueryService<GetAgentBankInfosQuery, AgentBankInfo>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public GetAgentBankInfosQueryService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public IAsyncEnumerable<AgentBankInfo> QueryAsync(
            GetAgentBankInfosQuery query,
            CancellationToken cancellationToken = default)
            => _paymentServiceClient.GetAgentBankInfosAsync(query.AgentId);
    }
}
