using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Promotion.Core;
using TGS.Services.Promotion.Core.Models.Promotion;

namespace TGS.Services.BackOffice.Application.Commands.PromotionHandlers
{
    public class ChangePromotionStatusHandler : Handler<ChangePromotionStatusRequest, MessageResponse>
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPromotionService _service;
        private readonly ISettingsService _settingsService;

        public ChangePromotionStatusHandler(IPromotionService service, ISettingsService settingsService,
            IJsonSerializationProvider jsonSerializationProvider, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
            _settingsService = settingsService;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        public override async Task<MessageResponse> Handle(ChangePromotionStatusRequest request,
            CancellationToken cancellationToken)
        {
            var promotion = await _service.GetPromotionAsync(q => q.Id == request.Id);

            if (promotion == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Promotion.NotFound);
            }

            promotion.Status = request.Status;

            await _service.ChangePromotionStatusAsync(promotion);

            return CreateValidResponse<MessageResponse>();
        }
    }
}