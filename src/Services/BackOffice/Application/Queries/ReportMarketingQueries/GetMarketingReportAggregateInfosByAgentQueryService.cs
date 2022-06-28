using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

internal class GetMarketingReportAggregateInfosByAgentQueryService
    : IGetService<GetMarketingReportAggregateInfosByAgentQuery, MarketingReportResponse>
{
    public ValueTask<MarketingReportResponse> GetAsync(
        GetMarketingReportAggregateInfosByAgentQuery query,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
