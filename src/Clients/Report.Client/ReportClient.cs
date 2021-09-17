using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Clients.Report.Client.Models.Agent;
using TGS.Clients.Report.Client.Models.Market;
using TGS.Clients.Report.Client.Models.Player;
using TGS.Clients.Report.Client.Models.Round;
using TGS.Clients.Report.Client.Models.Vip;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client
{
    public interface IReportClient
    {
        Task<PagedResponse<VipModel>> GetVipReportAsync(GetVipReportRequest request);
        Task<PagedResponse<VipRebateModel>> GetVipRebateReportAsync(GetVipRebateReportRequest request);
        Task<PagedResponse<RoundModel>> GetRoundsAsync(GetRoundsRequest request);
        Task<GetRoundHistoryResponse> GetRoundHistoriesAsync(GetRoundHistoryRequest request);
        Task<PagedResponse<MarketModel>> GetMarketsAsync(GetMarketsRequest request);
        Task<PagedResponse<AgentContributionModel>> GetAgentContributionsAsync(GetAgentContributionsRequest request);
        Task<PagedResponse<PlayerContributionModel>> GetPlayerContributionsAsync(GetPlayerContributionsRequest request);
        Task<PagedResponse<TeamInfoModel>> GetAgentTeamInfoAsync(GetAgentTeamInfoRequest request);
        Task<PagedResponse<AgentDividendModel>> GetAgentDividendsAsync(GetAgentDividendsRequest request);

        Task<PagedResponse<AgentDividendDetailModel>> GetAgentDividendDetailsAsync(
            GetAgentDividendDetailsRequest request);

        Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(GetAgentPlayerWinLossRequest request);

        Task<PagedResponse<PlayerAccountChangeModel>> GetPlayerAccountChangesAsync(
            GetPlayerAccountChangesRequest request);

        Task<PagedResponse<AgentAccountChangeModel>> GetAgentAccountChangesAsync(
            GetAgentAccountChangesRequest request);

        Task<Stream> ExportMarketsAsync(ExportMarketsRequest request);
        Task<Stream> ExportPlayerContributionsAsync(ExportPlayerContributionsRequest request);
    }

    public class ReportClient : BaseApiClient, IReportClient
    {
        public ReportClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async Task<PagedResponse<VipModel>> IReportClient.GetVipReportAsync(GetVipReportRequest request)
        {
            var prefix = $"Report.{request.BrandId}.GetVipReport";
            var path = ClientPath.AppendWithProperties("/api/vipReport?", request);

            return await ExecuteJsonRequestAsync<GetVipReportRequest, PagedResponse<VipModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<VipRebateModel>> IReportClient.GetVipRebateReportAsync(
            GetVipRebateReportRequest request)
        {
            var prefix = $"Report.{request.BrandId}.GetVipRebateReport";
            var path = ClientPath.AppendWithProperties($"/api/vipReport/{request.Username}?", request);
            return await ExecuteJsonRequestAsync<GetVipRebateReportRequest, PagedResponse<VipRebateModel>>(
                HttpMethod.Get, path, request, logPrefix: prefix, options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<RoundModel>> IReportClient.GetRoundsAsync(GetRoundsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetRounds";
            var path = ClientPath.AppendWithProperties("/api/roundReport?", request);

            return await ExecuteJsonRequestAsync<GetRoundsRequest, PagedResponse<RoundModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<GetRoundHistoryResponse> IReportClient.GetRoundHistoriesAsync(GetRoundHistoryRequest request)
        {
            var prefix = $"Report.RoundId.{request.RoundId}.GetRound";
            var path = ClientPath.AppendWithProperties($"/api/roundReport/{request.RoundId}?", request);

            return await ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<MarketModel>> IReportClient.GetMarketsAsync(GetMarketsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetMarkets";
            var path = ClientPath.AppendWithProperties("/api/marketReport?", request);

            return await ExecuteJsonRequestAsync<GetMarketsRequest, PagedResponse<MarketModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<AgentContributionModel>> IReportClient.GetAgentContributionsAsync(
            GetAgentContributionsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAgentContributions";
            var path = ClientPath.AppendWithProperties("/api/agentReport/contribution?", request);
            return await ExecuteJsonRequestAsync<GetAgentContributionsRequest, PagedResponse<AgentContributionModel>>(
                HttpMethod.Get, path, request, logPrefix: prefix, options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<PlayerContributionModel>> IReportClient.GetPlayerContributionsAsync(
            GetPlayerContributionsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetPlayerContributions";
            var path = ClientPath.AppendWithProperties("/api/playerReport/contribution?", request);

            return await ExecuteJsonRequestAsync<GetPlayerContributionsRequest, PagedResponse<PlayerContributionModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<TeamInfoModel>> IReportClient.GetAgentTeamInfoAsync(
            GetAgentTeamInfoRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentTeamInfo";
            var path = ClientPath.AppendWithProperties($"/api/agentReport/{request.AgentId}/teamInfo?", request);

            return await ExecuteJsonRequestAsync<GetAgentTeamInfoRequest, PagedResponse<TeamInfoModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<AgentDividendModel>> IReportClient.GetAgentDividendsAsync(
            GetAgentDividendsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAgentDividends";
            var path = ClientPath.AppendWithProperties("/api/agentReport/dividends?", request);

            return await ExecuteJsonRequestAsync<GetAgentDividendsRequest, PagedResponse<AgentDividendModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<AgentPlayerWinLossModel>> IReportClient.GetAgentPlayerWinLossAsync(
            GetAgentPlayerWinLossRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentPlayerWinLoss";
            var path = ClientPath.AppendWithProperties($"/api/agentReport/{request.AgentId}/winLoss?", request);

            return await ExecuteJsonRequestAsync<GetAgentPlayerWinLossRequest, PagedResponse<AgentPlayerWinLossModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<AgentDividendDetailModel>> IReportClient.GetAgentDividendDetailsAsync(
            GetAgentDividendDetailsRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentDividendDetails";
            var path = ClientPath.AppendWithProperties("/api/agentReport/dividendDetails?", request);

            return await ExecuteJsonRequestAsync<GetAgentDividendDetailsRequest, PagedResponse<AgentDividendDetailModel>
            >(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<PlayerAccountChangeModel>> IReportClient.GetPlayerAccountChangesAsync(
            GetPlayerAccountChangesRequest request)
        {
            var prefix = $"Report.PlayerCode.{request.PlayerCode}.GetPlayerAccountChanges";
            var path = ClientPath.AppendWithProperties("/api/playerReport/accountChanges?", request);

            return await ExecuteJsonRequestAsync<GetPlayerAccountChangesRequest, PagedResponse<PlayerAccountChangeModel>
            >(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<AgentAccountChangeModel>> IReportClient.GetAgentAccountChangesAsync(
            GetAgentAccountChangesRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentAccountChanges";
            var path = ClientPath.AppendWithProperties("/api/agentReport/accountChanges?", request);

            return await ExecuteJsonRequestAsync<GetAgentAccountChangesRequest, PagedResponse<AgentAccountChangeModel>
            >(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task<Stream> ExportMarketsAsync(ExportMarketsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/marketReport/export?", request);

            return await GetFileAsync(path);
        }

        public async Task<Stream> ExportPlayerContributionsAsync(ExportPlayerContributionsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/playerReport/export?", request);

            return await GetFileAsync(path);
        }
    }
}