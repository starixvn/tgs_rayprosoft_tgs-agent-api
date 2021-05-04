using System.Net.Http;
using System.Threading.Tasks;
using TGS.Clients.Promotion.Client.Models.PromotionDispatch;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client
{
    public interface IPromotionDispatchClient
    {
        Task<PagedResponse<PromotionDispatchModel>> GetPromotionDispatchesAsync(GetPromotionDispatchesRequest request);
        Task<MessageResponse> DispatchPromotionAsync(DispatchPromotionBonusRequest request);
    }

    public class PromotionDispatchClient : BaseApiClient, IPromotionDispatchClient
    {
        public PromotionDispatchClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async Task<MessageResponse> IPromotionDispatchClient.DispatchPromotionAsync(
            DispatchPromotionBonusRequest request)
        {
            const string prefix = "Promotion.DispatchPromotionBonus";
            return await ExecuteJsonRequestAsync<DispatchPromotionBonusRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotionDispatch", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<PromotionDispatchModel>> IPromotionDispatchClient.GetPromotionDispatchesAsync(
            GetPromotionDispatchesRequest request)
        {
            var prefix = $"PromotionDispatch.{request.BrandId}.GetPromotionDispatches";
            var path = ClientPath.AppendWithProperties("/api/promotionDispatch?", request);

            return await ExecuteJsonRequestAsync<GetPromotionDispatchesRequest, PagedResponse<PromotionDispatchModel>>(
                HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}