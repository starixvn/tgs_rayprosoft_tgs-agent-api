using System.Net.Http;
using System.Threading.Tasks;
using Report.Client.Models.Promotion;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;

namespace Report.Client
{
    public interface IPromotionSettlementClient
    {
        Task<MessageResponse> CreatePromotionSettleDataDailyAsync(CreatePromotionBonusDailyRequest request);
        Task<MessageResponse> CreatePromotionSettleDataWeeklyAsync(CreatePromotionBonusWeeklyRequest request);
        Task<MessageResponse> CreatePromotionSettleDataMonthlyAsync(CreatePromotionBonusMonthlyRequest request);
    }

    public class PromotionSettlementClient : BaseApiClient, IPromotionSettlementClient
    {
        public PromotionSettlementClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async Task<MessageResponse> IPromotionSettlementClient.CreatePromotionSettleDataDailyAsync(
            CreatePromotionBonusDailyRequest request)
        {
            const string prefix = "Report.CreatePromotionSettleDataDaily";
            return await ExecuteJsonRequestAsync<CreatePromotionBonusDailyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion/settleDataDaily", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IPromotionSettlementClient.CreatePromotionSettleDataWeeklyAsync(
            CreatePromotionBonusWeeklyRequest request)
        {
            var prefix = $"Report.{request.From}.{request.To}.CreatePromotionSettleDataWeekly";
            return await ExecuteJsonRequestAsync<CreatePromotionBonusWeeklyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion/settleDataWeekly", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IPromotionSettlementClient.CreatePromotionSettleDataMonthlyAsync(
            CreatePromotionBonusMonthlyRequest request)
        {
            var prefix = $"Report.{request.From}.{request.To}.CreatePromotionSettleDataMonthly";
            return await ExecuteJsonRequestAsync<CreatePromotionBonusMonthlyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion/settleDataMonthly", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}