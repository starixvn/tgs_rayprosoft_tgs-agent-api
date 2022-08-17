using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.MarketingReport.Client;
using Tgs.Paging;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.ReportMarketing;
using TGS.Services.BackOffice.Application.Queries.AgentHandlers;
using TGS.Services.BackOffice.Application.Queries.ReportMarketingQueries;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class MarketingReportController : BaseContextController
{
    [HttpGet("firstDepositDetail")]
    [Description("Get first deposit details report")]
    public Task<PagedResponse<MarketingFirstDepositDetailReportModel>> GetMarketingFirstDepositDetailsAsync(
        [FromServices]
        IGetService<GetMarketingFirstDepositDetailQuery, PagedResponse<MarketingFirstDepositDetailReportModel>>
            marketsService,
        [FromQuery] GetMarketingFirstDepositDetailReportRequest request,
        [FromQuery] PageableRequest pagingRequest)
    {
        var date = DateOnly.FromDateTime(request.Date.ToOffset(TimeSpan.FromHours(8D)).DateTime);

        var query = new GetMarketingFirstDepositDetailQuery(
            request.BrandId,
            date,
            request.AgentUsername,
            pagingRequest);

        return marketsService.GetAsync(query).AsTask();
    }

    [HttpGet("gamingDetail")]
    [Description("Get gaming details report")]
    public async Task<PagedResponse<MarketingGamingDetailReportModel>> GetMarketingGamingDetailsAsync(
        [FromServices] IGetService<GetMarketingGamingDetailReportRequest, PagedResponse<MarketingGamingDetailReportModel>> marketsService,
        [FromQuery] GetMarketingGamingDetailReportRequest request)
        => await marketsService
            .GetAsync(request)
            .ConfigureAwait(false);

    [HttpGet]
    [Description("Get marketing report")]
    public async Task<MarketingReportResponse> GetMarketingListAsync(
        [FromServices] IQueryService<AgentMarketingReportQuery, DailyAggregateInfo> queryService,
        [FromServices] IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics> queryPaymentService,
        [FromServices] IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentsService,
        [FromQuery] GetMarketingListReportRequest request,
        [FromQuery] PageableRequest pagingRequest)
    {
        var start = DateOnly.FromDateTime(request.From.ToOffset(TimeSpan.FromHours(8D)).DateTime);
        var end = DateOnly.FromDateTime(request.To.ToOffset(TimeSpan.FromHours(8D)).DateTime);

        var agentQuery = queryAgentsService
            .QueryAsync(new AllAgentsByBrandQuery(request.BrandId), HttpContext.RequestAborted);

        var agentDict = await agentQuery
            .ToDictionaryAsync(
                a => a.UserName,
                a => a.AgentId,
                HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var paymentData = queryPaymentService.QueryAsync(new(request.BrandId), HttpContext.RequestAborted)
            .Where(item => item.ReportDate >= start
                && item.ReportDate < end);
        var marketingData = queryService.QueryAsync(new(request.BrandId), HttpContext.RequestAborted)
            .Where(item => item.ReportDate >= start
                && item.ReportDate < end);

        var dateArray = Enumerable.Range(0, end.DayNumber - start.DayNumber)
            .Select(n => start.AddDays(n));

        var dataQuery = from date in dateArray.ToAsyncEnumerable()
                        from agentId in agentDict.Values.ToAsyncEnumerable()
                        join marketingItem in marketingData
                            on new { ReportDate = date, AgentId = agentId } equals new { marketingItem.ReportDate, marketingItem.AgentId } into gm
                        from marketingItem in gm.DefaultIfEmpty()
                        join paymentItem in paymentData
                            on new { ReportDate = date, AgentId = agentId } equals new { paymentItem.ReportDate, paymentItem.AgentId } into gp
                        from paymentItem in gp.DefaultIfEmpty()
                        select new
                        {
                            ReportDate = date,
                            AgentId = marketingItem?.AgentId ?? paymentItem.AgentId,
                            RegisterCount = marketingItem?.RegisterCount ?? 0,
                            FirstDepositPlayers = paymentItem?.FirstDepositPlayers ?? 0,
                            GamingCount = marketingItem?.GamingCount ?? 0
                        };

        if (!string.IsNullOrWhiteSpace(request.AgentUsername))
        {
            if (!agentDict.TryGetValue(request.AgentUsername, out var agentId))
                return new MarketingReportResponse
                {
                    TotalCount = 0,
                    Summary = new MarketingReportTotalModel(),
                    Items = Array.Empty<MarketingReportModel>()
                };

            dataQuery = dataQuery.Where(item => item.AgentId == agentId);
        }

        var paging = dataQuery.CreatePaging();

        var propertyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(MarketingReportModel.ReportDate)] = nameof(DailyAggregateInfo.ReportDate),
            [nameof(MarketingReportModel.GamingCount)] = nameof(DailyAggregateInfo.GamingCount),
            [nameof(MarketingReportModel.RegisterCount)] = nameof(DailyAggregateInfo.RegisterCount)
        };

        if (!string.IsNullOrWhiteSpace(pagingRequest.SortBy) && propertyMap.ContainsKey(pagingRequest.SortBy))
            paging.SortByPropertyName(propertyMap[pagingRequest.SortBy], pagingRequest.IsDesc);

        paging.Page = pagingRequest.Page;
        paging.RowCount = pagingRequest.Rows;

        var totalCount = await dataQuery
            .CountAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);
        var summary = await dataQuery.AggregateAsync(
            new MarketingReportTotalModel(),
            (seed, statistics) =>
            {
                seed.RegisterCount += statistics.RegisterCount;
                seed.FirstDepositCount += statistics.FirstDepositPlayers;
                seed.GamingCount += statistics.GamingCount;

                return seed;
            },
            HttpContext.RequestAborted).ConfigureAwait(false);

        dataQuery = dataQuery
            .ApplyOrderBy(paging)
            .ApplyPaging(paging);

        var agentNameDict = await agentQuery
            .ToDictionaryAsync(
                a => a.AgentId,
                a => a.UserName,
                HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new MarketingReportResponse
        {
            TotalCount = totalCount,
            Summary = summary,
            Items = await dataQuery
                .Select(a => new MarketingReportModel
                {
                    ReportDate = a.ReportDate,
                    AgentUsername = agentNameDict[a.AgentId],
                    RegisterCount = a.RegisterCount,
                    FirstDepositCount = a.FirstDepositPlayers,
                    GamingCount = a.GamingCount
                })
                .ToArrayAsync(HttpContext.RequestAborted)
                .ConfigureAwait(false)
        };
    }

    [HttpGet("registerDetail")]
    [Description("Get register details report")]
    public async Task<PagedResponse<MarketingRegisterDetailReportModel>> GetMarketingRegisterDetailsAsync(
        [FromServices] IQueryService<AgentPlayerRegisterDetailQuery, PlayerRegisterDetail> queryService,
        [FromServices] IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentsService,
        [FromQuery] GetMarketingRegisterDetailReportRequest request)
    {
        var agentQuery = queryAgentsService
           .QueryAsync(new AllAgentsByBrandQuery(request.BrandId), HttpContext.RequestAborted);

        var agentDict = await agentQuery
            .ToDictionaryAsync(
                a => a.UserName,
                a => a.AgentId,
                HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var agentId = agentDict[request.AgentUsername];

        var query = new AgentPlayerRegisterDetailQuery(request.BrandId, request.ReportDate);
        var statisticsData = await queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Where(item => item.AgentId == agentId)
            .ToArrayAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var dataQuery = statisticsData.AsQueryable();

        var paging = new Paging<PlayerRegisterDetail>();

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            paging.SortByPropertyName(request.SortBy, request.IsDesc);

        paging.Page = request.Page;
        paging.RowCount = request.Rows;

        var totalCount = statisticsData.Length;

        dataQuery = dataQuery
            .ApplyOrderBy(paging)
            .ApplyPaging(paging);

        return new PagedResponse<MarketingRegisterDetailReportModel>
        {
            TotalCount = totalCount,
            Items = dataQuery
                .Select(item => new MarketingRegisterDetailReportModel
                {
                    Username = item.PlayerName,
                    LastLoginOn = item.LastLoginOn,
                    LoginCount = item.LoginCount,
                    RegisteredOn = item.RegisteredOn
                })
                .ToArray()
        };
    }
}
