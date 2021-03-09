using System.Globalization;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using Valhalla.Services;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/player/password")]
    [ApiController]
    public class PlayerPasswordController : ControllerBase
    {
        /// <summary>
        /// Player changes password, player login is needed when use this
        /// </summary>
        [HttpPut]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [Consumes(MediaTypeNames.Application.Json)]
        public Task<ChangePasswordResponse> ChangePasswordAsync(
            [FromServices] IMediator mediator,
            [FromBody] ChangePasswordRequest request) => mediator.Send(request);


        [HttpPost("forgot")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]

        public async Task ForgotPasswordAsync(
            [FromServices] ICommandService<ForgotPasswordCommand> forgotPasswordService,
            [FromBody] ForgotPasswordRequest request)
        {
            await forgotPasswordService
                .ExecuteAsync(new ForgotPasswordCommand(CultureInfo.CurrentUICulture, request.Phone))
                .ConfigureAwait(false);
        }
    }
}
