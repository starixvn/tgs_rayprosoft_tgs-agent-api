using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Auth;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AuthController : ControllerBase
    {
        [HttpPost("jwt")]
        [AllowAnonymous]

        public async Task<AuthResponse> AuthenticateAsync(
            [FromServices] IGetService<GetAuthQuery, AuthInfo> getAuthQueryService,
            [FromBody] AuthRequest request)
        {
            var query = new GetAuthQuery(
                request.ApiKey,
                Consts.ClaimName.Currency,
                request.Currency,
                Consts.Site);

            var info = await getAuthQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            return await this.FailAsync<AuthResponse, AuthResponse>(
                StatusCodes.Status503ServiceUnavailable,
                new AuthResponse()
                {
                    Token = info.Token,
                    ExpiredOn = info.ExpiredOn
                });
        }
    }
}
