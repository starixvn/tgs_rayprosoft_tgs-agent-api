using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Services.Promotion.Handlers
{
    public class UpdatePromotionHandler : Handler<UpdatePromotionRequest, MessageResponse>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPromotionRepository _service;
        private readonly ISettingsService _settingsService;

        public UpdatePromotionHandler(IPromotionRepository service, ISettingsService settingsService,
            IJsonSerializationProvider jsonSerializationProvider, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
            _settingsService = settingsService;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<MessageResponse> Handle(UpdatePromotionRequest request,
            CancellationToken cancellationToken)
        {
            var promotion = await _service.GetPromotionAsync(q => q.Id == request.Id);

            if (promotion == null)
            {
                return CreateInvalidResponse<MessageResponse>(TGS.Services.Promotion.Properties.Promotion.NotFound);
            }

            if (await _service.ExistsAsync(x => request.Id != x.Id && x.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(TGS.Services.Promotion.Properties.Promotion.NameExisted);
            }

            var promotionsJson = await _settingsService.GetSettingAsync(x => x.Key == SettingConsts.PromotionType);
            var promotionTypeGroups =
                _jsonSerializationProvider.DeserializeFromString<PromotionTypeGroup[]>(promotionsJson.Value);

            if (promotionTypeGroups.SelectMany(x => x.Types).All(x => x.Name != request.PromotionType))
            {
                return CreateInvalidResponse<MessageResponse>(TGS.Services.Promotion.Properties.Promotion.PromotionTypeNotFound);
            }

            var promotionType = promotionTypeGroups.SelectMany(x => x.Types)
                .First(x => x.Name == request.PromotionType);

            promotion.SettlementIntervalType = promotionType.SettlementIntervalType;

            await _service.UpdatePromotionAsync(promotion, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
