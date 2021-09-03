using System.Linq;
using System.Threading.Tasks;
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

            if (authAttr == null || !authAttr.NeedAuthorize && authAttr.AllowNullOrEmpty)
            {
                return;
            }
            var ignoreAttr = context.ActionDescriptor.EndpointMetadata.OfType<CheckEnabledAttribute>().LastOrDefault();

            var playerTokenData = context.HttpContext.Items.GetPlayerTokenData();

            var playerCode = playerTokenData.PlayerCode;

            var status = await _getService.GetAsync(new GetPlayerStatusQuery(playerCode));

            if (status == PlayerStatus.Disabled && ignoreAttr != null)
            {
                context.Result = new ObjectResult(
                    new FailResponse()
                    {
                        Error = (int)TgsApiError.UserBlocked,
                        ErrorMessage = string.Empty,
                        ErrorMessageKey = "UserBlocked"
                    })
                {
                    StatusCode = 403
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
                    StatusCode = 403
                };
            }
        }
    }
}