using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Round
{
    internal class GetRoundQueryHandler : Handler<GetRoundHistoryRequest, GetRoundHistoryResponse>
    {
        private readonly IPluggalbeGameProviderApiClient _pluggableApiClient;
        private readonly IPluggableOperations _pluggableOperations;

        public GetRoundQueryHandler(IPluggalbeGameProviderApiClient pluggableApiClient,
            IPluggableOperations pluggableOperations, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _pluggableApiClient = pluggableApiClient;
            _pluggableOperations = pluggableOperations;
        }

        public override async Task<GetRoundHistoryResponse> Handle(
            GetRoundHistoryRequest request,
            CancellationToken cancellationToken)
        {
            var url = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(request.GameProviderCode);
            return await _pluggableApiClient.GetGameProviderRoundHistory(url, request);
        }
    }
}