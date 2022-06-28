using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

internal class GetMarketingReportAggregateInfosQueryService
    : IGetService<GetMarketingReportAggregateInfosQuery, MarketingReportResponse>
{
    public ValueTask<MarketingReportResponse> GetAsync(GetMarketingReportAggregateInfosQuery query,
        CancellationToken cancellationToken = new CancellationToken())
        => throw new NotImplementedException();
}
