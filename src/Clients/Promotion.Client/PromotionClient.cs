using System;
using System.Net.Http;
using System.Threading.Tasks;
using TGS.Clients.Promotion.Client.Models.Promotion;
using TGS.Core.Infrastructures.ApiClient;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Messages;

namespace TGS.Clients.Promotion.Client
{
    public class PromotionClient : BaseApiClient, IPromotionClient
    {
        public PromotionClient(HttpClient client, IApiClientLogger logger,
            IJsonSerializationProvider json)
            : base(client, logger, json)
        {
        }

        public async Task<MessageResponse> ChangePromotionStatusAsync(UpdatePromotionStatusRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.PromotionId}.ChangeStatus";
            return await ExecuteJsonRequestAsync<UpdatePromotionStatusRequest, MessageResponse>(HttpMethod.Put,
                $"/api/promotion/{request.PromotionId}/{request.Status}", request, logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CheckCanJoinDepositPromotionAsync(Guid brandId, string playerCode,
            Guid promotionId)
        {
            var prefix = $"Promotion.PromotionId.{promotionId}.CheckCanJoinDepositPromotion";
            return await ExecuteJsonRequestAsync<JoinPromotionRequest, bool>(HttpMethod.Get,
                $"/api/promotion/{promotionId}/check/{brandId}/{playerCode}", null, logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request)
        {
            var prefix = $"Promotion.BrandId.{request.BrandId}.CreatePromotion";
            return await ExecuteJsonRequestAsync<CreatePromotionRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion", request, logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagedResponse<PromotionModel>> GetPromotionsAsync(GetPromotionsRequest request)
        {
            var prefix = $"Promotion.BrandId.{request.BrandId}.GetPromotions";
            var path = ClientPath.AppendWithProperties("/api/promotion?", request);

            return await ExecuteJsonRequestAsync<GetPromotionsRequest, PagedResponse<PromotionModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task<MessageResponse> JoinAsync(JoinPromotionRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.PromotionId}.JoinPromotion";
            return await ExecuteJsonRequestAsync<JoinPromotionRequest, MessageResponse>(HttpMethod.Post,
                $"/api/promotion/{request.PromotionId}/addPlayer/{request.PlayerCode}", request, logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }

        public async Task TagPlayerDepositAsync(string playerCode)
        {
            var prefix = $"Promotion.Deposit.{playerCode}";

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/promotion/deposit/{playerCode}");

            request.Headers.Remove("Accept");
            request.Headers.Add("Accept", "application/json");

            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.Add("Content-Type", "application/json");

            var response = await SendAsync(request, prefix).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async Task<MessageResponse> UpdatePromotionAsync(UpdatePromotionRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.Id}.UpdatePromotion";
            return await ExecuteJsonRequestAsync<UpdatePromotionRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion", request, logPrefix: prefix,
                options: new() { PropertyNameCaseInsensitive = true });
        }
    }
}
