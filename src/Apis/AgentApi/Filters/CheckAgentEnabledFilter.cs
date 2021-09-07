using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Agent.Services.Agent;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Filters
{
    public class CheckAgentEnabledFilter : IAsyncAuthorizationFilter
    {
        private readonly IGetService<GetAgentStatusQuery, AgentStatus> _getService;

        public CheckAgentEnabledFilter(IGetService<GetAgentStatusQuery, AgentStatus> getService)
        {
            _getService = getService ?? throw new ArgumentNullException(nameof(getService));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authAttr = context.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAgentTokenAttribute>()
                .LastOrDefault();

            if (authAttr == null || authAttr.AllowNullOrEmpty)
            {
                return;
            }

            var agentCheckEnabledAttr = context.ActionDescriptor.EndpointMetadata.OfType<CheckAgentEnabledAttribute>()
                .LastOrDefault();

            var agentTokenData = context.HttpContext.Items.GetAgentTokenData();

            var agentId = agentTokenData.AgentId;

            var status = await _getService.GetAsync(new GetAgentStatusQuery(agentId));

            if (status == AgentStatus.Disabled && agentCheckEnabledAttr != null)
            {
                context.Result = new ObjectResult(
                    new TgsResponse
                    {
                        Error = (int)TgsApiError.UserBlocked,
                        ErrorMessage = string.Empty,
                        ErrorMessageKey = "UserBlocked"
                    }) {StatusCode = StatusCodes.Status403Forbidden};
            }
            else if (status == AgentStatus.InBlacklist)
            {
                context.Result = new ObjectResult(
                    new TgsResponse
                    {
                        Error = (int)TgsApiError.UserBanned,
                        ErrorMessage = string.Empty,
                        ErrorMessageKey = "AccessDenied"
                    }) {StatusCode = StatusCodes.Status403Forbidden};
            }
        }
    }
}