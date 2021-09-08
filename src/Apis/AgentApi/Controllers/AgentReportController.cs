using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Clients.Report.Client;
using TGS.Clients.Report.Client.Models.Agent;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AgentReportController : ControllerBase
    {
        private readonly IReportClient _client;
        private readonly IMediator _mediator;

        public AgentReportController(IMediator mediator, IReportClient client)
        {
            _mediator = mediator;
            _client = client;
        }

        /// <summary>
        ///     Get agent contributions
        /// </summary>
        [HttpGet("contributions")]
        [Description("Get contributions")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentContributionModel>> GetContributionsAsync(
            [FromQuery] GetContributionsRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var contributionRequest = new GetAgentContributionsRequest
            {
                BrandId = brandId,
                AgentId = agentTokenData.AgentId,
                IncludeAgents = true,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentContributionsAsync(contributionRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentContributionModel>()
            };
        }

        /// <summary>
        ///     Get agent dividends
        /// </summary>
        [HttpGet("dividends")]
        [Description("Get dividends")]
        [Consumes(MediaTypeNames.Application.Json)]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentDividendModel>> GetDividendsAsync(
            [FromQuery] GetDividendsRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var dividendRequest = new GetAgentDividendsRequest
            {
                BrandId = brandId,
                AgentId = agentTokenData.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await _client.GetAgentDividendsAsync(dividendRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentDividendModel>()
            };
        }

        [HttpGet("dividendDetails")]
        [Description("Get agent dividend details")]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentDividendDetailModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetDividendDetailsRequest request)
        {
            var dividendRequest = new GetAgentDividendDetailsRequest
            {
                AgentId = request.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };
            var response = await _client.GetAgentDividendDetailsAsync(dividendRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentDividendDetailModel>()
            };
        }
    }
}