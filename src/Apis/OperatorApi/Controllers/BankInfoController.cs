using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Json;
using TGS.Services.Operator.Services.Withdraw;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankInfoController : ControllerBase
    {
        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task CreateBankInfoAsync(
            [FromServices] ICommandService<CreateBankInfoCommand> createService,
            [FromServices] IOptions<JsonOptions> options,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            CreateBankInfoRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerData = HttpContext.Items.GetPlayerTokenData();

            var command = new CreateBankInfoCommand(
                brandId,
                playerData.PlayerCode,
                request.PaymentChannelId,
                request.BankType,
                jsonSerializationProvider.SerializeToString(request.Detail, options.Value.JsonSerializerOptions));

            await createService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpDelete("{bankInfoId}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task DeleteBankInfoAsync(
            [FromServices] ICommandService<DeleteBankInfoCommand> deleteService,
            Guid bankInfoId)
        {
            var playerData = HttpContext.Items.GetPlayerTokenData();

            var command = new DeleteBankInfoCommand(
                playerData.PlayerCode,
                bankInfoId);

            await deleteService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public IAsyncEnumerable<BankInfoResponse> GetPlayerBankInfosAsync(
            [FromServices] IQueryService<GetPlayerBankInfosQuery, PlayerBankInfo> queryService)
        {
            var playerData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetPlayerBankInfosQuery(playerData.PlayerCode);

            return queryService.QueryAsync(query)
                .Select(info => new BankInfoResponse
                {
                    Id = info.Id,
                    PaymentChannelId = info.PaymentChannelId,
                    BankType = info.BankType,
                    Detail = info.Detail
                });
        }
    }
}
