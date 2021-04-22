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

        Task<MessageResponse> IPromotionSettlementClient.CreatePromotionSettleDataDailyAsync(
            CreatePromotionBonusDailyRequest request)
        {
            var prefix = "Report.CreatePromotionSettleDataDaily";
            return ExecuteJsonRequestAsync<CreatePromotionBonusDailyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion/settleDataDaily", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<MessageResponse> IPromotionSettlementClient.CreatePromotionSettleDataWeeklyAsync(
            CreatePromotionBonusWeeklyRequest request)
        {
            var prefix = $"Report.{request.From}.{request.To}.CreatePromotionSettleDataWeekly";
            return ExecuteJsonRequestAsync<CreatePromotionBonusWeeklyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion/settleDataWeekly", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        Task<MessageResponse> IPromotionSettlementClient.CreatePromotionSettleDataMonthlyAsync(
            CreatePromotionBonusMonthlyRequest request)
        {
            var prefix = $"Report.{request.From}.{request.To}.CreatePromotionSettleDataMonthly";
            return ExecuteJsonRequestAsync<CreatePromotionBonusMonthlyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion/settleDataMonthly", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}