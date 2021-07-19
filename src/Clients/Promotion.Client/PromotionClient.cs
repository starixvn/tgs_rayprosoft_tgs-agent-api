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
        private readonly HttpClient _client;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public PromotionClient(
            HttpClient client,
            IApiClientLogger logger,
            IJsonSerializationProvider jsonSerializationProvider)
            : base(client, logger, jsonSerializationProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        public async Task<MessageResponse> ChangePromotionStatusAsync(UpdatePromotionStatusRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.PromotionId}.ChangeStatus";
            return await ExecuteJsonRequestAsync<UpdatePromotionStatusRequest, MessageResponse>(HttpMethod.Put,
                $"/api/promotion/{request.PromotionId}/{request.Status}", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task<bool> CheckCanJoinDepositPromotionAsync(Guid brandId, string playerCode,
            Guid promotionId, decimal amount)
        {
            var response = await _client
                .GetAsync($"/api/promotion/{promotionId}/check/{brandId}/{playerCode}/{amount}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return _jsonSerializationProvider.DeserializeFromString<bool>(responseContent);
        }

        public async Task<MessageResponse> CreatePromotionAsync(CreatePromotionRequest request)
        {
            var prefix = $"Promotion.BrandId.{request.BrandId}.CreatePromotion";
            return await ExecuteJsonRequestAsync<CreatePromotionRequest, MessageResponse>(HttpMethod.Post,
                "/api/promotion", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task<PagedResponse<PromotionModel>> GetPromotionsAsync(GetPromotionsRequest request)
        {
            var prefix = $"Promotion.BrandId.{request.BrandId}.GetPromotions";
            var path = ClientPath.AppendWithProperties("/api/promotion?", request);

            return await ExecuteJsonRequestAsync<GetPromotionsRequest, PagedResponse<PromotionModel>>(HttpMethod.Get,
                path, request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task<MessageResponse> JoinAsync(JoinPromotionRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.PromotionId}.JoinPromotion";
            return await ExecuteJsonRequestAsync<JoinPromotionRequest, MessageResponse>(HttpMethod.Post,
                $"/api/promotion/{request.PromotionId}/addPlayer/{request.PlayerCode}", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task RevertPromotionAsync(Guid promotionId, decimal amount)
        {
            var prefix = $"Promotion.Revert.{promotionId}";

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/promotion/{promotionId}/{amount}");

            request.Headers.Remove("Accept");
            request.Headers.Add("Accept", "application/json");

            var content = new StringContent(string.Empty);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            request.Content = content;

            var response = await SendAsync(request, prefix).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async Task TagPlayerDepositAsync(string playerCode)
        {
            var prefix = $"Promotion.Deposit.{playerCode}";

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/promotion/deposit/{playerCode}");

            request.Headers.Remove("Accept");
            request.Headers.Add("Accept", "application/json");

            var content = new StringContent(string.Empty);
            content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            request.Content = content;

            var response = await SendAsync(request, prefix).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async Task<MessageResponse> UpdatePromotionAsync(UpdatePromotionRequest request)
        {
            var prefix = $"Promotion.PromotionId.{request.Id}.UpdatePromotion";
            return await ExecuteJsonRequestAsync<UpdatePromotionRequest, MessageResponse>(HttpMethod.Put,
                $"/api/promotion/{request.Id}", request, logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task<PagedResponse<PromotionModel>> GetValidPromotionsAsync(GetValidPromotionsRequest request)
        {
            var prefix = $"Promotion.BrandId.{request.BrandId}.GetValidPromotions";
            var path = ClientPath.AppendWithProperties("/api/promotion/valid?", request);

            return await ExecuteJsonRequestAsync<GetValidPromotionsRequest, PagedResponse<PromotionModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }

        public async Task<PagedResponse<JoinedPromotionModel>> GetJoinedPromotionsAsync(
            GetJoinedPromotionsRequest request)
        {
            var prefix = $"Promotion.PlayerCode.{request.PlayerCode}.GetJoinedPromotions";
            var path = ClientPath.AppendWithProperties("/api/promotion/joined?", request);

            return await ExecuteJsonRequestAsync<GetJoinedPromotionsRequest, PagedResponse<JoinedPromotionModel>>(
                HttpMethod.Get,
                path,
                request,
                logPrefix: prefix,
                options: new() {PropertyNameCaseInsensitive = true});
        }
    }
}