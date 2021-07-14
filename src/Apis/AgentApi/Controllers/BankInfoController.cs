using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.Json;
using TGS.ServiceUtility;
using TGS.Services.Agent.Services.BankInfos;
using Tgs.Payment.Clients;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankInfoController : ControllerBase
    {
        [HttpPost]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task CreateBankInfoAsync(
            [FromServices] ICommandService<CreateBankInfoCommand> createService,
            [FromServices] IOptions<JsonOptions> options,
            [FromServices] IJsonSerializationProvider jsonSerializationProvider,
            CreateBankInfoRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var agentData = HttpContext.Items.GetAgentTokenData();

            var command = new CreateBankInfoCommand(
                brandId,
                agentData.AgentId,
                request.PaymentChannelId,
                request.BankType,
                jsonSerializationProvider.SerializeToString(request.Detail, options.Value.JsonSerializerOptions));

            await createService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpDelete("{bankInfoId}")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task DeleteBankInfoAsync(
            [FromServices] ICommandService<DeleteBankInfoCommand> deleteService,
            Guid bankInfoId)
        {
            var agentData = HttpContext.Items.GetAgentTokenData();

            var command = new DeleteBankInfoCommand(
                agentData.AgentId,
                bankInfoId);

            await deleteService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpGet]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public IAsyncEnumerable<BankInfoResponse> GetPlayerBankInfosAsync(
            [FromServices] IQueryService<GetAgentBankInfosQuery, AgentBankInfo> queryService)
        {
            var agentData = HttpContext.Items.GetAgentTokenData();

            var query = new GetAgentBankInfosQuery(agentData.AgentId);

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
