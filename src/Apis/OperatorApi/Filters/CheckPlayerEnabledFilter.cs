using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operator.Services.Player;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Filters
{
    public class CheckPlayerEnabledFilter : IAsyncAuthorizationFilter
    {
        private readonly IGetService<GetPlayerStatusQuery, PlayerStatus> _getService;

        public CheckPlayerEnabledFilter(
            IGetService<GetPlayerStatusQuery, PlayerStatus> getService)
        {
            _getService = getService ?? throw new System.ArgumentNullException(nameof(getService));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authAttr = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizePlayerTokenAttribute>().LastOrDefault();

            if (authAttr == null || authAttr.AllowNullOrEmpty)
            {
                return;
            }

            if(authAttr.NeedAuthorize == false)
            {
                return;
            }

            var playerCheckEnabledAttr = context.ActionDescriptor.EndpointMetadata.OfType<PlayerCheckEnabledAttribute>().LastOrDefault();

            var playerTokenData = context.HttpContext.Items.GetPlayerTokenData();

            var playerCode = playerTokenData.PlayerCode;

            var status = await _getService.GetAsync(new GetPlayerStatusQuery(playerCode));

            if (status == PlayerStatus.Disabled && playerCheckEnabledAttr != null)
            {
                context.Result = new ObjectResult(
                    new FailResponse()
                    {
                        Error = (int)TgsApiError.UserBlocked,
                        ErrorMessage = string.Empty,
                        ErrorMessageKey = "UserBlocked"
                    })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
            else if (status == PlayerStatus.InBlacklist)
            {
                context.Result = new ObjectResult(
                    new FailResponse()
                    {
                        Error = (int)TgsApiError.UserBanned,
                        ErrorMessage = string.Empty,
                        ErrorMessageKey = "AccessDenied"
                    })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}