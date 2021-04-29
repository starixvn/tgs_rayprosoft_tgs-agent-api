using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Messages;
using TGS.Core.Models.Settings;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.PromotionCore;
using TGS.Services.PromotionCore.Models.PromotionDispatch;
using TGS.Services.PromotionCore.Models.Promotions;

namespace TGS.Services.BackOffice.Application.Commands.PromotionHandlers
{
    public class DispatchPromotionBonusHandler : Handler<DispatchPromotionBonusRequest, MessageResponse>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPromotionService _service;
        private readonly ISettingsService _settingsService;

        public DispatchPromotionBonusHandler(IPromotionService service, ISettingsService settingsService,
            IJsonSerializationProvider jsonSerializationProvider, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
            _settingsService = settingsService;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<MessageResponse> Handle(DispatchPromotionBonusRequest request,
            CancellationToken cancellationToken)
        {
            if (await _service.ExistsAsync(x => x.Name == request.Name))
            {
                return CreateInvalidResponse<MessageResponse>(Promotion.NameExisted);
            }

            var promotionsJson = await _settingsService.GetSettingAsync(x => x.Key == SettingConsts.PromotionType);
            var promotionTypeGroups =
                _jsonSerializationProvider.DeserializeFromString<PromotionTypeGroup[]>(promotionsJson.Value);

            if (promotionTypeGroups.SelectMany(x => x.Types).All(x => x.Name != request.PromotionType))
            {
                return CreateInvalidResponse<MessageResponse>(Promotion.PromotionTypeNotFound);
            }

            var promotionType = promotionTypeGroups.SelectMany(x => x.Types)
                .First(x => x.Name == request.PromotionType);

            await _service.CreatePromotionAsync(request, promotionType);

            return CreateValidResponse<MessageResponse>();
        }
    }
}