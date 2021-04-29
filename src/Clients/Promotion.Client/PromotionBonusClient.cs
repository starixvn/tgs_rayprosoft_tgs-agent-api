using System.Net.Http;
using System.Threading.Tasks;
using Promotion.Client.Models.PromotionBonus;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;

namespace Promotion.Client
{
    public interface IPromotionBonusClient
    {
        Task<MessageResponse> CreatePromotionSettleDataDailyAsync(CreatePromotionBonusDailyRequest request);
        Task<MessageResponse> CreatePromotionSettleDataWeeklyAsync(CreatePromotionBonusWeeklyRequest request);
        Task<MessageResponse> CreatePromotionSettleDataMonthlyAsync(CreatePromotionBonusMonthlyRequest request);
    }

    public class PromotionBonusClient : BaseApiClient, IPromotionBonusClient
    {
        public PromotionBonusClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async Task<MessageResponse> IPromotionBonusClient.CreatePromotionSettleDataDailyAsync(
            CreatePromotionBonusDailyRequest request)
        {
            const string prefix = "Report.CreatePromotionSettleDataDaily";
            return await ExecuteJsonRequestAsync<CreatePromotionBonusDailyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotionBonus/settleDataDaily", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IPromotionBonusClient.CreatePromotionSettleDataWeeklyAsync(
            CreatePromotionBonusWeeklyRequest request)
        {
            var prefix = $"Report.{request.From}.{request.To}.CreatePromotionSettleDataWeekly";
            return await ExecuteJsonRequestAsync<CreatePromotionBonusWeeklyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotionBonus/settleDataWeekly", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IPromotionBonusClient.CreatePromotionSettleDataMonthlyAsync(
            CreatePromotionBonusMonthlyRequest request)
        {
            var prefix = $"Report.{request.From}.{request.To}.CreatePromotionSettleDataMonthly";
            return await ExecuteJsonRequestAsync<CreatePromotionBonusMonthlyRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotionBonus/settleDataMonthly", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}