using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.BackendOffice.Controllers.Messages.Player;
using TGS.Apis.BackendOffice.Extensions;
using TGS.Apis.BackendOffice.Models.Player;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Commands.PlayerIdentity;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerIdentityController : BaseContextController
    {
        [HttpGet("{playerCode}/identity", Name = "GetPlayerIdentityAuthenticationForm")]
        [Description("Get a player identity authentication form")]
        public async ValueTask<PlayerIdentityForm> GetPlayerIdentityAuthenticationFormAsync(
            [FromServices] IGetService<GetPlayerIdentityFormQuery, PlayerIdentityInfo> getService,
            string playerCode)
        {
            var query = new GetPlayerIdentityFormQuery(playerCode);

            var info = await getService.GetAsync(query).ConfigureAwait(false);

            return new PlayerIdentityForm
            {
                FormId = info.Id,
                RealName = info.RealName,
                IdNumber = info.IdNumber,
                AuthData = JsonDocument.Parse(info.Data),
                CreatedOn = info.CreatedOn,
                Status = info.Status,
                Remark = info.Remark,
                ReviewedOn = info.ReviewedOn,
                ReviewedUserName = info.ReviewedUserName
            };
        }

        [HttpPost("{formId}", Name = "ReviewDepositForm")]
        public async Task<MessageResponse> ReviewDepositFormAsync(
           [FromServices] ICommandService<ReviewPlayerIdentityFormCommand> reviewCommandService,
           Guid formId,
           [FromBody] PlayerIdentityReviewRequest reviewResponse)
        {
            var command = new ReviewPlayerIdentityFormCommand(
                reviewResponse.BrandId,
                formId,
                reviewResponse.ReviewResponse.ToReviewState(),
                reviewResponse.Remark,
                UserId,
                Username);

            await reviewCommandService
                .ExecuteAsync(command)
                .ConfigureAwait(false);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
