using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models.Enums;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.Agent;

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
        /// Get account changes
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        [HttpGet("accountchanges")]
        [Description("Get account changes")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentAccountChangeModel>> GetAccountChangesAsync(
            [FromQuery] GetAccountChangesRequest request,
            [FromQuery] PagingRequest pagingRequest,
            [FromQuery] Guid id)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var accountChangesRequest = new GetAgentAccountChangesRequest
            {
                AgentId = id,
                BrandId = brandId,
                From = request.From,
                To = request.To,
                TransactionType = (short?)request.TransactionType,
                Page = pagingRequest.Page,
                Rows = pagingRequest.Rows,
                OrderBy = pagingRequest.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = pagingRequest.SortBy,
                ShowAll = pagingRequest.ShowAll
            };

            var response = await _client.GetAgentAccountChangesAsync(accountChangesRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentAccountChangeModel>()
            };
        }

        /// <summary>
        /// Get account changes transaction types
        /// </summary>
        /// <returns></returns>
        [HttpGet("accountchanges/transactiontypes")]
        [Description("Get account changes transaction types")]
        public Task<Dictionary<int, string>> GetAccountChangesTransactionTypesAsync()
        {
            var allowType = new[]
            {
                TransactionType.Withdrawal,
                TransactionType.CancelWithdrawal,
                TransactionType.Adjust,
                TransactionType.Dividend
            };

            var dict = Enum.GetValues(typeof(TransactionType))
                .Cast<TransactionType>()
                .Where(x => allowType.Contains(x))
                .ToDictionary(t => (int)t, t => t.ToString());

            return Task.FromResult(dict);
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
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
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

        [HttpGet("dividendTeamDetails")]
        [Description("Get agent dividend team details")]
        [TgsResponseWrapper]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<AgentDividendTeamDetailModel>>
            GetAgentDividendDetailsAsync([FromQuery] GetDividendTeamDetailsRequest request)
        {
            var dividendRequest = new GetAgentDividendTeamDetailsRequest
            {
                AgentId = request.AgentId,
                From = request.From,
                To = request.To,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };
            var response = await _client.GetAgentDividendTeamDetailsAsync(dividendRequest);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<AgentDividendTeamDetailModel>()
            };
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
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
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
                OrderBy = request.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
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

        /// <summary>
        /// Get team Information
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        [HttpGet("teaminfo")]
        [Description("Get team Information")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<PagingResponse<TeamInfoModel>> GetTeamInformationAsync(
            [FromQuery] PagingRequest pagingRequest,
            [FromQuery] Guid id)
        {
            var request = new GetAgentTeamInfoRequest
            {
                AgentId = id,
                Page = pagingRequest.Page,
                Rows = pagingRequest.Rows,
                OrderBy = pagingRequest.IsDesc ? (short)SortOrder.Descending : (short)SortOrder.Ascending,
                SortBy = pagingRequest.SortBy,
                ShowAll = pagingRequest.ShowAll
            };

            var response = await _client.GetAgentTeamInfoAsync(request);

            return new()
            {
                TotalCount = response.TotalCount,
                Items = response.Items ?? Array.Empty<TeamInfoModel>()
            };
        }
    }
}