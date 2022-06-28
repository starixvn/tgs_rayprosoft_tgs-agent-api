using System;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Models.ReportMarket;

namespace TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;

public record GetMarketingReportAggregateInfosQuery(
    Guid BrandId,
    DateOnly StartDate,
    DateOnly EndDate,
    Paging<MarketReportModel> Paging);
