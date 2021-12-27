using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Controllers.Contraints;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent.ApplicationServices.PayPassowrd;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/pay/password")]
    [ApiController]
    public class PayPasswordController : ControllerBase
    {
        private readonly ILogger<PayPasswordController> _logger;

        public PayPasswordController(ILogger<PayPasswordController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPatch, HK28CheckContraint]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask ChangePayPasswordAsync(
            [FromServices] ICommandService<ChangePayPasswordCommand> changePayPasswordService,
            ChangePayPasswordRequest request)
        {
            try
            {
                var agentTokenData = HttpContext.Items.GetAgentTokenData();

                var command = new ChangePayPasswordCommand(
                    agentTokenData.AgentId,
                    request.Password,
                    request.NewPassword);

                return changePayPasswordService.ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change PayPassword occur error.");

                throw new InvalidOperationException();
            }
        }

        [HttpPatch(Order = 1)]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask ChangePayPasswordByValidateCodeAsync(
            [FromServices] ICommandService<ChangePayPasswordByValidateCodeCommand> changePayPasswordService,
            ChangePayPasswordByValidateCodeRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new ChangePayPasswordByValidateCodeCommand(
                agentTokenData.AgentId,
                request.NewPassword,
                request.ValidateCode,
                request.SecurityKey);

            return changePayPasswordService.ExecuteAsync(command);
        }

        [HttpPost]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetPayPasswordAsync(
            [FromServices] ICommandService<SetFirstPayPasswordCommand> setPayPasswordService,
            [FromBody] SetPayPasswordRequest request)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new SetFirstPayPasswordCommand(agentTokenData.AgentId, request.Password);

            await setPayPasswordService.ExecuteAsync(command).ConfigureAwait(false);
        }

        [HttpPost("validate-code")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public ValueTask<string> StartChangePayPasswordAsync(
            [FromServices] IGetService<StartChangePayPasswordCommand, string> startChangePayPasswordService)
        {
            var agentTokenData = HttpContext.Items.GetAgentTokenData();

            var command = new StartChangePayPasswordCommand(agentTokenData.AgentId, CultureInfo.CurrentUICulture);

            return startChangePayPasswordService.GetAsync(command);
        }
    }
}
