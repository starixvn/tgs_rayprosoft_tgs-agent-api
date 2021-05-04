using System.Net.Http;
using System.Threading.Tasks;
using TGS.Clients.Promotion.Client.Models.Promotion;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client
{
    public interface IPromotionClient
    {
        Task<PagedResponse<PromotionModel>> GetPromotionsAsync(GetPromotionsRequest request);
        Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request);
        Task<MessageResponse> UpdatePromotionAsync(UpdatePromotionRequest request);
        Task<MessageResponse> ChangePromotionStatusAsync(UpdatePromotionStatusRequest request);
    }

    public class PromotionClient : BaseApiClient, IPromotionClient
    {
        public PromotionClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        async Task<MessageResponse> IPromotionClient.CreatePromotionAsync(CreatePromotionRequest request)
        {
            var prefix = $"Promotion.BrandId.{request.BrandId}.CreatePromotion";
            return await ExecuteJsonRequestAsync<CreatePromotionRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<PagedResponse<PromotionModel>> IPromotionClient.GetPromotionsAsync(GetPromotionsRequest request)
        {
            var prefix = $"Promotion.{request.BrandId}.GetPromotions";
            var path = ClientPath.AppendWithProperties("/api/promotion?", request);

            return await ExecuteJsonRequestAsync<GetPromotionsRequest, PagedResponse<PromotionModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IPromotionClient.UpdatePromotionAsync(UpdatePromotionRequest request)
        {
            var prefix = $"Promotion.Id.{request.Id}.UpdatePromotion";
            return await ExecuteJsonRequestAsync<UpdatePromotionRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        async Task<MessageResponse> IPromotionClient.ChangePromotionStatusAsync(UpdatePromotionStatusRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.PromotionId}.UpdatePromotion";
            return await ExecuteJsonRequestAsync<UpdatePromotionStatusRequest, MessageResponse>(HttpMethod.Put,
                $"/api/promotion/{request.PromotionId}/{request.Status}", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}