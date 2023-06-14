using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using Tgs.ServiceUtility;
using TGS.Services.Agent.ApplicationServices.AgentWinLossReport;
using TGS.Services.Agent;
using System.Linq;
using TGS.Apis.AgentApi.Models.ReportAgentWinLoss;

namespace TGS.Apis.AgentApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class AgentWinLossReportController : ControllerBase
{
    [HttpGet("breadcrumb")]
    [Description("Get Agent WinLoss Breadcrumb")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<IEnumerable<AgentWinLossBreadcrumbReportModel>>
        GetBreadcrumbAsync(
            [FromServices] IGetService<GetAgentWinLossBreadcrumbQuery, IEnumerable<AgentWinLossBreadcrumb>>
                agentWinLossBreadcrumbService,
            [FromQuery] GetAgentWinLossBreadcrumbReportRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var agentData = HttpContext.Items.GetAgentTokenData();

        var response = await agentWinLossBreadcrumbService
            .GetAsync(new GetAgentWinLossBreadcrumbQuery
            {
                BrandId = brandId,
                AgentId = agentData.AgentId,
                Username = request.Username,
                MemberType = request.MemberType,
                RoundId = request.RoundId
            })
            .ConfigureAwait(false);

        return response.Select(x => new AgentWinLossBreadcrumbReportModel
        {
            Username = x.Username,
            MemberType = x.MemberType,
            Id = x.Id
        });
    }

    [HttpGet("agentWinLoss")]
    [Description("Get Agent WinLoss")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<GetAgentWinLossReportResponse>
        GetBreadcrumbAsync(
            [FromServices] IGetService<GetAgentWinLossQuery, GetAgentWinLossResponse>
                agentWinLossService,
            [FromQuery] GetAgentWinLossReportRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();
        var agentData = HttpContext.Items.GetAgentTokenData();

        var query = new GetAgentWinLossQuery
        {
            BrandId = brandId,
            AgentId = agentData.AgentId,
            Username = request.Username,
            MemberType = request.MemberType,
            DownLine = request.DownLine,
            From = request.From,
            To = request.To,
        };

        var response = await agentWinLossService
            .GetAsync(query)
            .ConfigureAwait(false);

        return new GetAgentWinLossReportResponse
        {
            Items = response.Items?.Select(x => new AgentWinLossReportModel
            {
                Id = x.Id,
                Username = x.Username,
                MemberType = x.MemberType,
                RoundCount = x.RoundCount,
                Turnover = x.Turnover,
                WinLoss = x.WinLoss,
                AgentWinLoss = x.AgentWinLoss,
                CompanyWinLoss = x.CompanyWinLoss,
                Special = x.Special,
                AgentSpecial = x.AgentSpecial,
                CompanySpecial = x.CompanySpecial,
                AgentTotal = x.AgentTotal,
                CompanyTotal = x.CompanyTotal,
            }).ToArray(),
            Total = new AgentWinLossReportTotalModel
            {
                RoundCount = response.Total?.RoundCount ?? default,
                Turnover = response.Total?.Turnover ?? default,
                WinLoss = response.Total?.WinLoss ?? default,
                AgentWinLoss = response.Total?.AgentWinLoss ?? default,
                CompanyWinLoss = response.Total?.CompanyWinLoss ?? default,
                Special = response.Total?.Special ?? default,
                AgentSpecial = response.Total?.AgentSpecial ?? default,
                CompanySpecial = response.Total?.CompanySpecial ?? default,
                AgentTotal = response.Total?.AgentTotal ?? default,
                CompanyTotal = response.Total?.CompanyTotal ?? default,
            },
            TotalCount = response.TotalCount
        };
    }

    [HttpGet("agentPlayerSpecialDetail")]
    [Description("Get Agent Player Special Detail")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<AgentPlayerSpecialDetailReportModel>
        GetAgentPlayerSpecialDetailAsync(
            [FromServices] IGetService<GetAgentPlayerSpecialDetailQuery, AgentPlayerSpecialDetailModel>
                agentPlayerSpecialService,
            [FromQuery] GetAgentPlayerSpecialDetailReportRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();

        var query = new GetAgentPlayerSpecialDetailQuery
        {
            BrandId = brandId,
            AgentId = request.AgentId,
            From = request.From,
            To = request.To,
        };

        var response = await agentPlayerSpecialService
            .GetAsync(query)
            .ConfigureAwait(false);

        return new AgentPlayerSpecialDetailReportModel
        {
            Bonus = response.Bonus,
            BonusAdjust = response.BonusAdjust,
            ProductFee = response.ProductFee,
        };
    }

    [HttpGet("agentTotalDetail")]
    [Description("Get Agent Total Detail")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<IEnumerable<AgentTotalDetailReportModel>>
        GetAgentSpecialDetailAsync(
            [FromServices] IGetService<GetAgentTotalDetailQuery, IEnumerable<AgentTotalDetailModel>>
                agentSpecialService,
            [FromQuery] GetAgentTotalDetailReportRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();

        var query = new GetAgentTotalDetailQuery
        {
            BrandId = brandId,
            AgentId = request.AgentId,
            From = request.From,
            To = request.To,
        };

        var response = await agentSpecialService
            .GetAsync(query)
            .ConfigureAwait(false);

        return response.Select(x => new AgentTotalDetailReportModel
        {
            From = x.From,
            To = x.To,
            Percentage = x.Percentage,
            PlayerWinLoss = x.PlayerWinLoss,
            PlayerSpecial = x.PlayerSpecial,
            AgentWinLoss = x.AgentWinLoss,
            AgentSpecial = x.AgentSpecial,
            AgentTotal = x.AgentTotal,
        });
    }

    [HttpGet("companyTotalDetail")]
    [Description("Get Company Total Detail")]
    [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
    public async Task<IEnumerable<CompanyTotalDetailReportModel>>
        GetCompanySpecialDetailAsync(
            [FromServices] IGetService<GetCompanyTotalDetailQuery, IEnumerable<CompanyTotalDetailModel>>
                companySpecialService,
            [FromQuery] GetCompanyTotalDetailReportRequest request)
    {
        var brandId = HttpContext.Items.GetBrandId();

        var query = new GetCompanyTotalDetailQuery
        {
            BrandId = brandId,
            AgentId = request.AgentId,
            From = request.From,
            To = request.To,
        };

        var response = await companySpecialService
            .GetAsync(query)
            .ConfigureAwait(false);

        return response.Select(x => new CompanyTotalDetailReportModel
        {
            From = x.From,
            To = x.To,
            Percentage = x.Percentage,
            PlayerWinLoss = x.PlayerWinLoss,
            PlayerSpecial = x.PlayerSpecial,
            CompanyWinLoss = x.CompanyWinLoss,
            CompanySpecial = x.CompanySpecial,
            CompanyTotal = x.CompanyTotal,
        });
    }
}