using System.Net.Http;
using System.Threading.Tasks;
using Report.Client.Models.Agent;
using Report.Client.Models.Market;
using Report.Client.Models.Player;
using Report.Client.Models.Round;
using Report.Client.Models.Vip;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;

namespace Report.Client
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
        Task<PagedResponse<AgentDividendModel>> GetAgentBonusAsync(GetAgentDividendRequest request);
        Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(GetAgentPlayerWinLossRequest request);
    }

    public class ReportClient : BaseApiClient, IReportClient
    {
        public ReportClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        Task<PagedResponse<VipModel>> IReportClient.GetVipReportAsync(GetVipReportRequest request)
        {
            var prefix = $"Report.{request.BrandId}.GetVipReport";
            var path = Path.AppendWithProperties("/api/vipReport?", request);

            return ExecuteJsonRequestAsync<GetVipReportRequest, PagedResponse<VipModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<VipRebateModel>> IReportClient.GetVipRebateReportAsync(GetVipRebateReportRequest request)
        {
            var prefix = $"Report.{request.BrandId}.GetVipRebateReport";
            var path = Path.AppendWithProperties($"/api/vipReport/{request.Username}?", request);

            return ExecuteJsonRequestAsync<GetVipRebateReportRequest, PagedResponse<VipRebateModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<RoundModel>> IReportClient.GetRoundsAsync(GetRoundsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetRounds";
            var path = Path.AppendWithProperties("/api/round?", request);

            return ExecuteJsonRequestAsync<GetRoundsRequest, PagedResponse<RoundModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<GetRoundHistoryResponse> IReportClient.GetRoundHistoriesAsync(GetRoundHistoryRequest request)
        {
            var prefix = $"Report.RoundId.{request.RoundId}.GetRound";
            var path = Path.AppendWithProperties($"/api/round/{request.RoundId}?", request);

            return ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(HttpMethod.Get,
                $"/api/round/{request.RoundId}", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<MarketModel>> IReportClient.GetMarketsAsync(GetMarketsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetMarkets";
            var path = Path.AppendWithProperties("/api/marketReport?", request);

            return ExecuteJsonRequestAsync<GetMarketsRequest, PagedResponse<MarketModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<AgentContributionModel>> IReportClient.GetAgentContributionsAsync(
            GetAgentContributionsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAgentContributions";
            var path = Path.AppendWithProperties("/api/agentReport/contribution?", request);

            return ExecuteJsonRequestAsync<GetAgentContributionsRequest, PagedResponse<AgentContributionModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<PlayerContributionModel>> IReportClient.GetPlayerContributionsAsync(
            GetPlayerContributionsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetPlayerContributions";
            var path = Path.AppendWithProperties("/api/playerReport/contribution?", request);

            return ExecuteJsonRequestAsync<GetPlayerContributionsRequest, PagedResponse<PlayerContributionModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<TeamInfoModel>> IReportClient.GetAgentTeamInfoAsync(
            GetAgentTeamInfoRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentTeamInfo";
            var path = Path.AppendWithProperties($"/api/agentReport/{request.AgentId}/teamInfo?", request);

            return ExecuteJsonRequestAsync<GetAgentTeamInfoRequest, PagedResponse<TeamInfoModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<AgentDividendModel>> IReportClient.GetAgentBonusAsync(GetAgentDividendRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAgentBonus";
            var path = Path.AppendWithProperties("/api/agentReport/bonus?", request);

            return ExecuteJsonRequestAsync<GetAgentDividendRequest, PagedResponse<AgentDividendModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<PagedResponse<AgentPlayerWinLossModel>> IReportClient.GetAgentPlayerWinLossAsync(
            GetAgentPlayerWinLossRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentPlayerWinLoss";
            var path = Path.AppendWithProperties($"/api/agentReport/{request.AgentId}/winLoss?", request);

            return ExecuteJsonRequestAsync<GetAgentPlayerWinLossRequest, PagedResponse<AgentPlayerWinLossModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}