using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Clients.Report.Client.Models.Agent;
using TGS.Clients.Report.Client.Models.GameProvider;
using TGS.Clients.Report.Client.Models.LicenseeMarket;
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
        Task<PagedResponse<OpenRoundModel>> GetOpenRoundsAsync(GetOpenRoundsRequest request);

        Task<GetRoundHistoryResponse> GetRoundHistoriesAsync(GetRoundHistoryRequest request);

        Task<PagedResponse<MarketModel>> GetMarketsAsync(GetMarketsRequest request);

        Task<GetAdditionalExpensesResponse> GetAdditionalExpensesAsync(GetAdditionalExpensesRequest request);
        Task<PagedResponse<AgentContributionModel>> GetAgentContributionsAsync(GetAgentContributionsRequest request);

        Task<PagedResponse<PlayerContributionModel>> GetPlayerContributionsAsync(GetPlayerContributionsRequest request);

        Task<PagedResponse<TeamInfoModel>> GetAgentTeamInfoAsync(GetAgentTeamInfoRequest request);

        Task<PagedResponse<AgentDividendModel>> GetAgentDividendsAsync(GetAgentDividendsRequest request);

        Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(GetAgentPlayerWinLossRequest request);

        Task<PagedResponse<AgentDividendDetailModel>> GetAgentDividendDetailsAsync(
            GetAgentDividendDetailsRequest request);

        Task<PagedResponse<AgentDividendTeamDetailModel>> GetAgentDividendTeamDetailsAsync(
            GetAgentDividendTeamDetailsRequest request);

        Task<PagedResponse<PlayerAccountChangeModel>> GetPlayerAccountChangesAsync(
            GetPlayerAccountChangesRequest request);

        Task<PagedResponse<PlayersAccountChangeModel>> GetPlayersAccountChangesAsync(
            GetPlayersAccountChangesRequest request);

        Task<PagedResponse<AgentAccountChangeModel>> GetAgentAccountChangesAsync(GetAgentAccountChangesRequest request);
        Task<PagedResponse<GameProviderRoundsModel>> GetGameProviderRoundsAsync(GetGameProviderRoundsRequest request);

        Task<LicenseeMarketsResponse> GetLicenseeMarketsAsync(GetLicenseeMarketsRequest request);

        Task<Stream> ExportMarketsAsync(ExportMarketsRequest request);

        Task<Stream> ExportPlayerContributionsAsync(ExportPlayerContributionsRequest request);

        Task<Stream> ExportAgentContributionsAsync(ExportAgentContributionsRequest request);

        Task<Stream> ExportAgentDividendsAsync(ExportAgentDividendsRequest request);

        Task<Stream> ExportVipsAsync(ExportVipReportRequest request);

        Task<Stream> ExportAgentDividendsTrialCalculationAsync(ExportAgentDividendsTrialCalculationRequest request);

        Task<Stream> ExportLicenseeMarketsAsync(ExportLicenseeMarketsRequest request);
    }

    public class ReportClient : BaseApiClient, IReportClient
    {
        private readonly HttpClient _client;

        public ReportClient(
            HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
            => _client = client;

        public async Task<PagedResponse<VipModel>> GetVipReportAsync(GetVipReportRequest request)
        {
            var prefix = $"Report.{request.BrandId}.GetVipReport";
            var path = ClientPath.AppendWithProperties("/api/vipReport?", request);

            return await ExecuteJsonRequestAsync<GetVipReportRequest, PagedResponse<VipModel>>(HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<VipRebateModel>> GetVipRebateReportAsync(
            GetVipRebateReportRequest request)
        {
            var prefix = $"Report.{request.BrandId}.GetVipRebateReport";
            var path = ClientPath.AppendWithProperties($"/api/vipReport/{request.Username}?", request);

            return await ExecuteJsonRequestAsync<GetVipRebateReportRequest, PagedResponse<VipRebateModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<RoundModel>> GetRoundsAsync(GetRoundsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetRounds";
            var path = ClientPath.AppendWithProperties("/api/roundReport?", request);

            return await ExecuteJsonRequestAsync<GetRoundsRequest, PagedResponse<RoundModel>>(HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<OpenRoundModel>> GetOpenRoundsAsync(GetOpenRoundsRequest request)
        {
            var prefix = "Report.GetOpenRounds";
            var path = ClientPath.AppendWithProperties("/api/roundReport/openRounds?", request);

            return await ExecuteJsonRequestAsync<GetOpenRoundsRequest, PagedResponse<OpenRoundModel>>(HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<GetRoundHistoryResponse> GetRoundHistoriesAsync(GetRoundHistoryRequest request)
        {
            var prefix = $"Report.RoundId.{request.RoundId}.GetRound";
            var path = ClientPath.AppendWithProperties($"/api/roundReport/{request.RoundId}?", request);

            return await ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<MarketModel>> GetMarketsAsync(GetMarketsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetMarkets";
            var path = ClientPath.AppendWithProperties("/api/marketReport?", request);

            return await ExecuteJsonRequestAsync<GetMarketsRequest, PagedResponse<MarketModel>>(HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<AgentContributionModel>> GetAgentContributionsAsync(
            GetAgentContributionsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAgentContributions";
            var path = ClientPath.AppendWithProperties("/api/agentReport/contribution?", request);

            return await ExecuteJsonRequestAsync<GetAgentContributionsRequest, PagedResponse<AgentContributionModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<PlayerContributionModel>> GetPlayerContributionsAsync(
            GetPlayerContributionsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetPlayerContributions";
            var path = ClientPath.AppendWithProperties("/api/playerReport/contribution?", request);

            return await ExecuteJsonRequestAsync<GetPlayerContributionsRequest, PagedResponse<PlayerContributionModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<TeamInfoModel>> GetAgentTeamInfoAsync(
            GetAgentTeamInfoRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentTeamInfo";
            var path = ClientPath.AppendWithProperties($"/api/agentReport/{request.AgentId}/teamInfo?", request);

            return await ExecuteJsonRequestAsync<GetAgentTeamInfoRequest, PagedResponse<TeamInfoModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<AgentDividendModel>> GetAgentDividendsAsync(
            GetAgentDividendsRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAgentDividends";
            var path = ClientPath.AppendWithProperties("/api/agentReport/dividends?", request);

            return await ExecuteJsonRequestAsync<GetAgentDividendsRequest, PagedResponse<AgentDividendModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<AgentPlayerWinLossModel>> GetAgentPlayerWinLossAsync(
            GetAgentPlayerWinLossRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentPlayerWinLoss";
            var path = ClientPath.AppendWithProperties($"/api/agentReport/{request.AgentId}/winLoss?", request);

            return await ExecuteJsonRequestAsync<GetAgentPlayerWinLossRequest, PagedResponse<AgentPlayerWinLossModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<AgentDividendDetailModel>> GetAgentDividendDetailsAsync(
            GetAgentDividendDetailsRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentDividendDetails";
            var path = ClientPath.AppendWithProperties("/api/agentReport/dividendDetails?", request);

            return await ExecuteJsonRequestAsync<GetAgentDividendDetailsRequest, PagedResponse<AgentDividendDetailModel>
            >(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<PlayerAccountChangeModel>> GetPlayerAccountChangesAsync(
            GetPlayerAccountChangesRequest request)
        {
            var prefix = $"Report.PlayerCode.{request.PlayerCode}.GetPlayerAccountChanges";
            var path = ClientPath.AppendWithProperties("/api/playerReport/accountChanges?", request);

            return await ExecuteJsonRequestAsync<GetPlayerAccountChangesRequest, PagedResponse<PlayerAccountChangeModel>
            >(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<PlayersAccountChangeModel>> GetPlayersAccountChangesAsync(
            GetPlayersAccountChangesRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetPlayersAccountChanges";
            var path = ClientPath.AppendWithProperties("/api/playerReport/playersAccountChanges?", request);

            return await ExecuteJsonRequestAsync<GetPlayersAccountChangesRequest,
                PagedResponse<PlayersAccountChangeModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<AgentAccountChangeModel>> GetAgentAccountChangesAsync(
            GetAgentAccountChangesRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentAccountChanges";
            var path = ClientPath.AppendWithProperties("/api/agentReport/accountChanges?", request);

            return await ExecuteJsonRequestAsync<GetAgentAccountChangesRequest, PagedResponse<AgentAccountChangeModel>
            >(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<LicenseeMarketsResponse> GetLicenseeMarketsAsync(GetLicenseeMarketsRequest request)
        {
            var prefix = $"Report.LicenseeId.{request.LicenseeId}.GetLicenseeMarkets";

            var keyValues = new Dictionary<string, string>();
            foreach (var propertyInfo in request
                         .GetType()
                         .GetProperties()
                         .Where(x => x != null && !x.Name.Contains("CurrencyCodes",
                             StringComparison.InvariantCultureIgnoreCase))
                         .ToArray())
            {
                var value = propertyInfo.GetValue(request);

                if (value == null)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(DateTimeOffset)
                    || propertyInfo.PropertyType == typeof(DateTimeOffset?))
                {
                    value = ((DateTimeOffset?)value)?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.sssZ");
                }

                keyValues.Add(propertyInfo.Name, value.ToString());
            }

            var parameters = string.Join("&", keyValues.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            var currencyCodes = request.CurrencyCodes
                ?.Select(currencyCode => string.Format("{0}={1}", "currencyCodes", currencyCode)).ToArray();

            var currencyCodeParameters = currencyCodes == null ? string.Empty : string.Join("&", currencyCodes);

            parameters += string.IsNullOrEmpty(parameters) ? currencyCodeParameters : $"&{currencyCodeParameters}";

            var path = $"/api/LicenseeMarketReport?{parameters}";

            return await ExecuteJsonRequestAsync<GetLicenseeMarketsRequest, LicenseeMarketsResponse>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<Stream> ExportMarketsAsync(ExportMarketsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/marketReport/exportMarkets?", request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Stream> ExportPlayerContributionsAsync(ExportPlayerContributionsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/playerReport/exportPlayerContributions?", request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Stream> ExportAgentContributionsAsync(ExportAgentContributionsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/agentReport/exportAgentContributions?", request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Stream> ExportAgentDividendsAsync(ExportAgentDividendsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/agentReport/exportAgentDividends?", request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Stream> ExportVipsAsync(ExportVipReportRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/vipReport/exportVips?", request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Stream> ExportAgentDividendsTrialCalculationAsync(
            ExportAgentDividendsTrialCalculationRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/agentReport/exportAgentDividendsTrialCalculation?",
                request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<Stream> ExportLicenseeMarketsAsync(ExportLicenseeMarketsRequest request)
        {
            var prefix = $"Report.LicenseeId.{request.LicenseeId}.ExportLicenseeMarkets";

            var keyValues = new Dictionary<string, string>();
            foreach (var propertyInfo in request
                         .GetType()
                         .GetProperties()
                         .Where(x => x != null && !x.Name.Contains("CurrencyCodes",
                             StringComparison.InvariantCultureIgnoreCase))
                         .ToArray())
            {
                var value = propertyInfo.GetValue(request);

                if (value == null)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(DateTimeOffset)
                    || propertyInfo.PropertyType == typeof(DateTimeOffset?))
                {
                    value = ((DateTimeOffset?)value)?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.sssZ");
                }

                keyValues.Add(propertyInfo.Name, value.ToString());
            }

            var parameters = string.Join("&", keyValues.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));

            var currencyCodes = request.CurrencyCodes
                ?.Select(currencyCode => string.Format("{0}={1}", "currencyCodes", currencyCode)).ToArray();

            var currencyCodeParameters = currencyCodes == null ? string.Empty : string.Join("&", currencyCodes);

            parameters += string.IsNullOrEmpty(parameters) ? currencyCodeParameters : $"&{currencyCodeParameters}";

            var path = $"/api/LicenseeMarketReport/exportLicenseeMarkets?{parameters}";

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, path);

            httpRequest.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

            var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }

        public async Task<PagedResponse<GameProviderRoundsModel>> GetGameProviderRoundsAsync(
            GetGameProviderRoundsRequest request)
        {
            var path = ClientPath.AppendWithProperties("/api/gameProviderReport?", request);

            return await ExecuteJsonRequestAsync<GetGameProviderRoundsRequest, PagedResponse<GameProviderRoundsModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: "Report.GetGameProviderReport",
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<AgentDividendTeamDetailModel>> GetAgentDividendTeamDetailsAsync(
            GetAgentDividendTeamDetailsRequest request)
        {
            var prefix = $"Report.AgentId.{request.AgentId}.GetAgentDividendTeamDetails";
            var path = ClientPath.AppendWithProperties("/api/agentReport/dividendTeamDetails?", request);

            return await ExecuteJsonRequestAsync<GetAgentDividendTeamDetailsRequest,
                PagedResponse<AgentDividendTeamDetailModel>
            >(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<GetAdditionalExpensesResponse> GetAdditionalExpensesAsync(
            GetAdditionalExpensesRequest request)
        {
            var prefix = $"Report.BrandId.{request.BrandId}.GetAdditionalExpenses";
            var path = ClientPath.AppendWithProperties("/api/marketReport/additionalExpenses?", request);

            return await ExecuteJsonRequestAsync<GetAdditionalExpensesRequest, GetAdditionalExpensesResponse>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }
    }
}