using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TGS.Apis.BackendOffice.Controllers.Messages.Banner;
using TGS.Core.Infrastructures.Json;
using TGS.Services.BackOffice.Application.Services;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : BaseContextController
    {
        [HttpPut("{brandId}")]
        public ValueTask CreateBannerAsync(
            [FromServices] ICommandService<CreateBannerCommand> createService,
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            Guid brandId,
            [FromBody] BannerModifyRequest request)
        {
            var jsonOptions = jsonOptionsAccessor.Value;

            var command = new CreateBannerCommand(
                brandId,
                request.Title,
                request.BeginTime,
                request.EndTime,
                jsonSerializationProvider.SerializeToString(
                    request.Localization,
                    jsonOptions.JsonSerializerOptions),
                this.Username);

            return createService.ExecuteAsync(command);
        }

        [HttpPatch("disable/{brandId}/{bannerId}")]
        public ValueTask DisableBannerAsync(
            [FromServices] ICommandService<SetIsEanbleBannerCommand> setService,
            Guid brandId,
            Guid bannerId)
        {
            var command = new SetIsEanbleBannerCommand(
                brandId,
                bannerId,
                false,
                Username);

            return setService.ExecuteAsync(command);
        }

        [HttpPatch("enable/{brandId}/{bannerId}")]
        public ValueTask EnableBannerAsync(
            [FromServices] ICommandService<SetIsEanbleBannerCommand> setService,
            Guid brandId,
            Guid bannerId)
        {
            var command = new SetIsEanbleBannerCommand(
                brandId,
                bannerId,
                true,
                Username);

            return setService.ExecuteAsync(command);
        }

        [HttpPost("{brandId}/{bannerId}")]
        public ValueTask ReviewBannerAsync(
            [FromServices] ICommandService<ReviewBannerCommand> reviewService,
            Guid brandId,
            Guid bannerId,
            [FromBody] ReviewBannerRequest request)
        {
            var command = new ReviewBannerCommand(
                brandId,
                bannerId,
                request.ReviewState,
                request.Remark,
                Username);

            return reviewService.ExecuteAsync(command);
        }

        [HttpPatch("{brandId}/{bannerId}")]
        public ValueTask UpdateBannerAsync(
            [FromServices] ICommandService<UpdateBannerCommand> updateService,
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            Guid brandId,
            Guid bannerId,
            [FromBody] BannerModifyRequest request)
        {
            var jsonOptions = jsonOptionsAccessor.Value;

            var command = new UpdateBannerCommand(
                brandId,
                bannerId,
                request.Title,
                request.BeginTime,
                request.EndTime,
                jsonSerializationProvider.SerializeToString(
                    request.Localization,
                    jsonOptions.JsonSerializerOptions),
                Username);

            return updateService.ExecuteAsync(command);
        }
    }
}
