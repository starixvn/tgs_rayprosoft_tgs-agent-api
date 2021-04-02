using System.Net.Http;
using System.Threading.Tasks;
using Report.Client.Models.Aggregate;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;

namespace Report.Client
{
    public interface IReportAggregateClient
    {
        Task<MessageResponse> CreatePlayerAggregateAsync(CreateUpdatePlayerAggregateModel request);
        Task<MessageResponse> ManualCreatePlayerAggregateAsync(ManualCreateUpdatePlayerAggregateModel request);
        Task<MessageResponse> CreateUpdateRoundAggregateAsync(CreateUpdateRoundAggregateModel request);
        Task<MessageResponse> ManualCreateUpdateRoundAggregateAsync(ManualCreateUpdateRoundAggregateModel request);
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
            return await ExecuteJsonRequestAsync<CreateUpdatePlayerAggregateModel, MessageResponse>(HttpMethod.Post,
                "/api/playerAggregate", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IReportAggregateClient.ManualCreatePlayerAggregateAsync(
            ManualCreateUpdatePlayerAggregateModel request)
        {
            var prefix = $"Report.{request.Date}.{request.Hour}.ManualCreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<ManualCreateUpdatePlayerAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/playerAggregate/manual", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IReportAggregateClient.CreateUpdateRoundAggregateAsync(
            CreateUpdateRoundAggregateModel request)
        {
            var prefix = "Report.CreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<CreateUpdateRoundAggregateModel, MessageResponse>(HttpMethod.Post,
                "/api/roundAggregate", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IReportAggregateClient.ManualCreateUpdateRoundAggregateAsync(
            ManualCreateUpdateRoundAggregateModel request)
        {
            var prefix = $"Report.{request.Date}.{request.Hour}.ManualCreateUpdatePlayerAggregate";
            return await ExecuteJsonRequestAsync<ManualCreateUpdateRoundAggregateModel, MessageResponse>(
                HttpMethod.Post,
                "/api/roundAggregate/manual", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}