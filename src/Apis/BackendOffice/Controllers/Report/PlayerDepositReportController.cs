using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Core.Models.Messages;
using TGS.Paging;
using TGS.Services.BackOffice.Application.Models.ReportPlayerDeposit;
using TGS.Services.BackOffice.Application.Queries.AgentHandlers;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
public class PlayerDepositReportController : BaseContextController
{
    [HttpGet]
    [Description("Get player deposits")]
    public async Task<PagedResponse<PlayerDepositReportModel>> GetPlayerDepositsAsync(
        [FromServices] IQueryService<AgentPlayerDepositQuery, AgentPlayerDepositStatistics> queryService,
        [FromServices] IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentsService,
        [FromQuery] GetPlayerDepositsReportRequest request)
    {
        var query = new AgentPlayerDepositQuery(request.BrandId);

        var data = queryService.QueryAsync(query, HttpContext.RequestAborted);

        var dateRange = (
            From: DateOnly.FromDateTime(request.From.ToOffset(TimeSpan.FromHours(8D)).DateTime),
            To: DateOnly.FromDateTime(request.To.ToOffset(TimeSpan.FromHours(8D)).DateTime));

        var dataQuery = data.Where(s => s.ReportDate >= dateRange.From
            && s.ReportDate < dateRange.To);

        var agentQuery = queryAgentsService
            .QueryAsync(new AllAgentsByBrandQuery(request.BrandId), HttpContext.RequestAborted);

        var agentDict = await agentQuery
            .ToDictionaryAsync(
                a => a.UserName,
                a => a.AgentId,
                HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(request.AgentUsername))
        {
            if (!agentDict.TryGetValue(request.AgentUsername, out var agentId))
                return new PagedResponse<PlayerDepositReportModel>
                {
                    TotalCount = 0,
                    Items = Array.Empty<PlayerDepositReportModel>()
                };

            dataQuery = dataQuery.Where(a => a.AgentId == agentId);
        }

        var paging = new Paging<AgentPlayerDepositStatistics>();

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            paging.SortByPropertyName(request.SortBy, request.IsDesc);

        paging.Page = request.Page;
        paging.RowCount = request.Rows;

        var totalCount = await dataQuery
            .CountAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);

        dataQuery = dataQuery.ApplyPaging(paging);

        var agentNameDict = await agentQuery
            .ToDictionaryAsync(
                a => a.AgentId,
                a => a.UserName,
                HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PagedResponse<PlayerDepositReportModel>
        {
            TotalCount = totalCount,
            Items = await dataQuery
                .Select(a => new PlayerDepositReportModel
                {
                    ReportDate = a.ReportDate,
                    AgentUserName = agentNameDict[a.AgentId],
                    DepositCount = a.DepositPlayers,
                    FirstDepositCount = a.FirstDepositPlayers,
                    NoSecondDepositCount = a.NoSecondDepositPlayers
                })
                .ToArrayAsync(HttpContext.RequestAborted)
                .ConfigureAwait(false)
        };
    }

    [HttpGet("playerFirstDepositDetails")]
    [Description("Get player first deposit details report")]
    public async Task<PagedResponse<PlayerDepositDetailReportModel>> GetPlayerFirstDepositDetailsAsync(
        [FromServices] IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem> queryService,
        [FromServices] IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentsService,
        [FromQuery] GetPlayerDepositDetailsReportRequest request)
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

        var query = new PlayerPaymentStatisticsDetailQuery(request.BrandId, request.Date);
        var statisticsData = await queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Where(item => item.AgentId == agentId
                && item.IsFirstDeposit == true)
            .ToArrayAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var dataQuery = statisticsData.AsQueryable();

        var paging = new Paging<PlayerDailyPaymentItem>();

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            paging.SortByPropertyName(request.SortBy, request.IsDesc);

        paging.Page = request.Page;
        paging.RowCount = request.Rows;

        var totalCount = statisticsData.Length;

        dataQuery = dataQuery.ApplyPaging(paging);

        return new PagedResponse<PlayerDepositDetailReportModel>
        {
            TotalCount = totalCount,
            Items = dataQuery
                .Select(item => new PlayerDepositDetailReportModel
                {
                    Username = item.PlayerName,
                    FirstDepositOn = item.FirstDepositOn,
                    FirstDepositAmount = item.FirstDepositAmount,
                    TotalDepositAmount = item.TotalDepositAmount,
                    TotalWithdrawAmount = item.TotalWithdrawAmount,
                    DepositSubtractWithdraw = item.TotalDepositAmount - item.TotalWithdrawAmount,
                    IpAddress = item.RegisterIp
                })
                .ToArray()
        };
    }

    [HttpGet("playerDailyDepositDetails")]
    public async Task<PagedResponse<PlayerDepositDetailReportModel>> GetPlayerDailyDepositDetailsAsync(
        [FromServices] IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem> queryService,
        [FromServices] IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentsService,
        [FromQuery] GetPlayerDepositDetailsReportRequest request)
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

        var query = new PlayerPaymentStatisticsDetailQuery(request.BrandId, request.Date);
        var statisticsData = await queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Where(item => item.AgentId == agentId
                && item.IsFirstDeposit == false)
            .ToArrayAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var dataQuery = statisticsData.AsQueryable();

        var paging = new Paging<PlayerDailyPaymentItem>();

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            paging.SortByPropertyName(request.SortBy, request.IsDesc);

        paging.Page = request.Page;
        paging.RowCount = request.Rows;

        var totalCount = statisticsData.Length;

        dataQuery = dataQuery.ApplyPaging(paging);

        return new PagedResponse<PlayerDepositDetailReportModel>
        {
            TotalCount = totalCount,
            Items = dataQuery
                .Select(item => new PlayerDepositDetailReportModel
                {
                    Username = item.PlayerName,
                    FirstDepositOn = item.FirstDepositOn,
                    FirstDepositAmount = item.FirstDepositAmount,
                    TotalDepositAmount = item.TotalDepositAmount,
                    TotalWithdrawAmount = item.TotalWithdrawAmount,
                    DepositSubtractWithdraw = item.TotalDepositAmount - item.TotalWithdrawAmount,
                    IpAddress = item.RegisterIp
                })
                .ToArray()
        };
    }

    [HttpGet("playerNoSecondDepositDetails")]
    public async Task<PagedResponse<PlayerDepositDetailReportModel>> GetPlayerNoSecondDepositDetailsAsync(
        [FromServices] IQueryService<PlayerPaymentStatisticsDetailQuery, PlayerDailyPaymentItem> queryService,
        [FromServices] IQueryService<AllAgentsByBrandQuery, AgentInfo> queryAgentsService,
        [FromQuery] GetPlayerNoSecondDepositDetailsReportRequest request)
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

        var query = new PlayerPaymentStatisticsDetailQuery(request.BrandId, request.Date);
        var statisticsData = await queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Where(item => item.AgentId == agentId
                && item.UnSecondDeposit == true)
            .ToArrayAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);

        var dataQuery = statisticsData.AsQueryable();

        var paging = new Paging<PlayerDailyPaymentItem>();

        if (!string.IsNullOrWhiteSpace(request.SortBy))
            paging.SortByPropertyName(request.SortBy, request.IsDesc);

        paging.Page = request.Page;
        paging.RowCount = request.Rows;

        var totalCount = statisticsData.Length;

        dataQuery = dataQuery.ApplyPaging(paging);

        return new PagedResponse<PlayerDepositDetailReportModel>
        {
            TotalCount = totalCount,
            Items = dataQuery
                .Select(item => new PlayerDepositDetailReportModel
                {
                    Username = item.PlayerName,
                    FirstDepositOn = item.FirstDepositOn,
                    FirstDepositAmount = item.FirstDepositAmount,
                    TotalDepositAmount = item.TotalDepositAmount,
                    TotalWithdrawAmount = item.TotalWithdrawAmount,
                    DepositSubtractWithdraw = item.TotalDepositAmount - item.TotalWithdrawAmount,
                    IpAddress = item.RegisterIp
                })
                .ToArray()
        };
    }
}
