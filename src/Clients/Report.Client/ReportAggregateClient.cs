using System.Net.Http;
using System.Threading.Tasks;
using TGS.Clients.Report.Client.Models.Agent;
using TGS.Clients.Report.Client.Models.Aggregate;
using TGS.Clients.Report.Client.Models.Player;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Report.Client
{
    public interface IReportAggregateClient
    {
        Task<MessageResponse> CreatePlayerRebateAsync(CreatePlayerRebateRequest request);
        Task<MessageResponse> CreateAgentDividendAsync(CreateAgentDividendsRequest request);
        Task<MessageResponse> CreatePlayerAggregateAsync(CreateUpdatePlayerAggregateModel request);
        Task<MessageResponse> ManualCreatePlayerAggregateAsync(ManualCreateUpdatePlayerAggregateModel request);
        Task<MessageResponse> CreateUpdateRoundAggregateAsync(CreateUpdateRoundAggregateModel request);
        Task<MessageResponse> ManualCreateUpdateRoundAggregateAsync(ManualCreateUpdateRoundAggregateModel request);
        Task<MessageResponse> CreateMarketAggregateAsync(CreateMarketAggregateModel request);
    }

    public class ReportAggregateClient : BaseApiClient, IReportAggregateClient
    {
        public ReportAggregateClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async Task<MessageResponse> IReportAggregateClient.CreatePlayerAggregateAsync(
            CreateUpdatePlayerAggregateModel request)
        {
            var prefix = "Report.CreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<CreateUpdatePlayerAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/playerAggregate", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }

        async Task<MessageResponse> IReportAggregateClient.ManualCreatePlayerAggregateAsync(
            ManualCreateUpdatePlayerAggregateModel request)
        {
            var prefix = $"Report.{request.Date}.{request.Hour}.ManualCreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<ManualCreateUpdatePlayerAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/playerAggregate/manual", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }

        async Task<MessageResponse> IReportAggregateClient.CreateUpdateRoundAggregateAsync(
            CreateUpdateRoundAggregateModel request)
        {
            var prefix = "Report.CreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<CreateUpdateRoundAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/roundAggregate", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }

        async Task<MessageResponse> IReportAggregateClient.ManualCreateUpdateRoundAggregateAsync(
            ManualCreateUpdateRoundAggregateModel request)
        {
            var prefix = $"Report.{request.Date}.{request.Hour}.ManualCreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<ManualCreateUpdateRoundAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/roundAggregate/manual", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }

        async Task<MessageResponse> IReportAggregateClient.CreatePlayerRebateAsync(CreatePlayerRebateRequest request)
        {
            var prefix = "Report.CreatePlayerRebate";
            return await ExecuteJsonRequestAsync<CreatePlayerRebateRequest, MessageResponse>(
                HttpMethod.Post,
                "/api/playerReport/rebates", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }

        async Task<MessageResponse> IReportAggregateClient.CreateAgentDividendAsync(CreateAgentDividendsRequest request)
        {
            var prefix = "Report.CreateAgentDividend";
            return await ExecuteJsonRequestAsync<CreateAgentDividendsRequest, MessageResponse>(
                HttpMethod.Post,
                "/api/agentReport/dividends", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }

        async Task<MessageResponse> IReportAggregateClient.CreateMarketAggregateAsync(
            CreateMarketAggregateModel request)
        {
            var prefix = "Report.CreateMarketAggregate";
            return await ExecuteJsonRequestAsync<CreateMarketAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/marketAggregate", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true}).ConfigureAwait(false);
        }
    }
}